-- Seed test user, weights, and calorie entries.
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

WITH params AS (
    SELECT
        DATE '2025-01-01' AS start_date,
        CURRENT_DATE AS end_date,
        82.0::numeric AS base_weight
),
days AS (
    SELECT
        day::date AS entry_date,
        (day::date - params.start_date)::int AS day_index,
        params.base_weight
    FROM params
    CROSS JOIN LATERAL generate_series(params.start_date, params.end_date, INTERVAL '1 day') AS day
),
weights_raw AS (
    SELECT
        entry_date,
        day_index,
        base_weight,
        base_weight
            + 2.5 * sin(2 * pi() * day_index / 365.0)
            + 1.2 * sin(2 * pi() * day_index / 45.0)
            + 0.6 * sin(2 * pi() * day_index / 9.0)
            + 0.3 * (random() - 0.5) AS raw_weight
    FROM days
),
weights AS (
    SELECT
        entry_date,
        day_index,
        base_weight,
        round(raw_weight::numeric, 1) AS weight_kg
    FROM weights_raw
),
user_row AS (
    INSERT INTO "Users" ("Id", "UserName", "PasswordHash")
    VALUES (gen_random_uuid(), 'testuser', '100000.vOz/w0Om09alpFHwTlHtng==.o8MBS/VDaq9+BvE1J33VuK4OBlt0EI9Lywxog/e3iTA=')
    ON CONFLICT ("UserName") DO NOTHING
    RETURNING "Id"
),
user_target AS (
    SELECT "Id" FROM user_row
    UNION ALL
    SELECT "Id" FROM "Users" WHERE "UserName" = 'testuser'
    LIMIT 1
),
insert_weights AS (
    INSERT INTO "WeightEntries" ("Id", "UserId", "EntryDate", "WeightKg")
    SELECT
        gen_random_uuid(),
        user_target."Id",
        weights.entry_date,
        weights.weight_kg::numeric(5,2)
    FROM weights
    CROSS JOIN user_target
    ON CONFLICT ("UserId", "EntryDate") DO NOTHING
    RETURNING 1
),
day_flags AS (
    SELECT
        weights.entry_date,
        weights.day_index,
        weights.base_weight,
        weights.weight_kg,
        (extract(dow from weights.entry_date) IN (0, 6)) AS is_weekend,
        random() AS holiday_roll,
        random() AS social_roll,
        random() AS lag_roll
    FROM weights
),
diet_blocks AS (
    SELECT
        block_index,
        random() < 0.30 AS is_diet
    FROM (
        SELECT DISTINCT floor(day_index / 21.0)::int AS block_index
        FROM day_flags
    ) blocks
),
day_profiles AS (
    SELECT
        day_flags.*,
        diet_blocks.is_diet,
        (3 + floor(lag_roll * 5))::int AS lag_days
    FROM day_flags
    JOIN diet_blocks ON diet_blocks.block_index = floor(day_flags.day_index / 21.0)::int
),
weight_for_calories AS (
    SELECT
        day_profiles.*,
        COALESCE(future.weight_kg, day_profiles.weight_kg) AS weight_for_calories
    FROM day_profiles
    LEFT JOIN weights AS future
        ON future.entry_date = day_profiles.entry_date + day_profiles.lag_days
),
meal_plan AS (
    SELECT
        weight_for_calories.*,
        parts.part,
        parts.part_roll,
        parts.extra_roll,
        CASE parts.part
            WHEN 'Morning' THEN CASE
                WHEN parts.part_roll < 0.10 THEN 0
                WHEN parts.part_roll < 0.80 THEN 1
                ELSE 2
            END
            WHEN 'Midday' THEN CASE
                WHEN parts.part_roll < 0.60 THEN 1
                WHEN parts.part_roll < 0.90 THEN 2
                ELSE 3
            END
            WHEN 'Evening' THEN CASE
                WHEN parts.part_roll < 0.65 THEN 1
                WHEN parts.part_roll < 0.90 THEN 2
                ELSE 1
            END
            WHEN 'Night' THEN CASE
                WHEN parts.part_roll < 0.90 THEN 0
                WHEN parts.part_roll < 0.98 THEN 1
                ELSE 2
            END
        END AS base_meal_count,
        (parts.part = 'Evening' AND parts.part_roll >= 0.90) AS is_large_meal
    FROM weight_for_calories
    CROSS JOIN LATERAL (
        SELECT
            part,
            random() AS part_roll,
            random() AS extra_roll
        FROM (VALUES ('Morning'), ('Midday'), ('Evening'), ('Night')) AS parts(part)
    ) AS parts
),
meals AS (
    SELECT
        meal_plan.*,
        meal_index.meal_no
    FROM meal_plan
    CROSS JOIN LATERAL (
        SELECT generate_series(
            1,
            meal_plan.base_meal_count
                + CASE
                    WHEN meal_plan.is_weekend
                        AND meal_plan.part IN ('Midday', 'Evening')
                        AND meal_plan.extra_roll < 0.20
                        THEN 1
                    ELSE 0
                END
        ) AS meal_no
    ) AS meal_index
),
calorie_rows AS (
    SELECT
        meals.entry_date,
        meals.part,
        meals.is_weekend,
        meals.is_diet,
        meals.social_roll,
        meals.holiday_roll,
        meals.weight_for_calories,
        meals.base_weight,
        meals.is_large_meal,
        CASE meals.part
            WHEN 'Morning' THEN ARRAY['Oatmeal', 'Toast', 'Eggs', 'Yogurt', 'Cereal', 'Pancakes', 'Fruit', 'Coffee & pastry']
            WHEN 'Midday' THEN ARRAY['Sandwich', 'Salad', 'Pasta', 'Pizza', 'Burger', 'Rice bowl', 'Soup', 'Wrap']
            WHEN 'Evening' THEN ARRAY['Chicken dinner', 'Fish & vegetables', 'Steak', 'Pasta', 'Stir-fry', 'Curry', 'Tacos']
            ELSE ARRAY['Chips', 'Ice cream', 'Cookies', 'Fruit', 'Nuts', 'Leftovers']
        END AS food_pool,
        random() AS normal_roll,
        random() AS food_roll,
        random() AS weight_roll
    FROM meals
),
calorie_values AS (
    SELECT
        calorie_rows.entry_date,
        calorie_rows.part,
        calorie_rows.food_pool[1 + floor(calorie_rows.food_roll * array_length(calorie_rows.food_pool, 1))::int] AS food_name,
        CASE calorie_rows.part
            WHEN 'Morning' THEN 400 + 80 * (sqrt(-2 * ln(greatest(calorie_rows.normal_roll, 0.000001))) * cos(2 * pi() * random()))
            WHEN 'Midday' THEN 600 + 120 * (sqrt(-2 * ln(greatest(calorie_rows.normal_roll, 0.000001))) * cos(2 * pi() * random()))
            WHEN 'Evening' THEN 700 + 140 * (sqrt(-2 * ln(greatest(calorie_rows.normal_roll, 0.000001))) * cos(2 * pi() * random()))
            ELSE 50 + 200 * power(random(), 1.8)
        END AS base_calories,
        CASE
            WHEN calorie_rows.weight_for_calories > calorie_rows.base_weight + 1
                THEN 1.05 + calorie_rows.weight_roll * 0.05
            WHEN calorie_rows.weight_for_calories < calorie_rows.base_weight - 1
                THEN 0.85 + calorie_rows.weight_roll * 0.05
            ELSE 1.0
        END AS weight_factor,
        CASE
            WHEN calorie_rows.holiday_roll < 0.03 THEN 2.0 + random()
            ELSE 1.0
        END AS holiday_factor,
        CASE
            WHEN calorie_rows.part IN ('Midday', 'Evening') AND calorie_rows.social_roll < 0.08 THEN 1.2
            ELSE 1.0
        END AS social_factor,
        CASE
            WHEN calorie_rows.is_weekend THEN 1.10
            ELSE 1.0
        END AS weekend_factor,
        CASE
            WHEN calorie_rows.is_diet THEN 0.80
            ELSE 1.0
        END AS diet_factor,
        CASE
            WHEN calorie_rows.is_large_meal THEN 1.25
            ELSE 1.0
        END AS large_meal_factor
    FROM calorie_rows
)
INSERT INTO "CalorieEntries" ("Id", "UserId", "EntryDate", "Calories", "FoodName", "PartOfDay", "Note")
SELECT
    gen_random_uuid(),
    user_target."Id",
    calorie_values.entry_date,
    greatest(50, round(calorie_values.base_calories
        * calorie_values.weight_factor
        * calorie_values.holiday_factor
        * calorie_values.social_factor
        * calorie_values.weekend_factor
        * calorie_values.diet_factor
        * calorie_values.large_meal_factor))::int AS calories,
    calorie_values.food_name,
    calorie_values.part,
    NULL
FROM calorie_values
CROSS JOIN user_target;
