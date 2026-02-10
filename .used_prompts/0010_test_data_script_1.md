# Test data script
Let's create a mechanism to apply sql scripts via nx/npm command.

## Db code placement
First of all I want to consolidate all database related things to a Db folder.
So all models and migrations and repositories folders end up there.
The I want a separate subfolder for sql scripts.

## Sql script application
I want to use EF built in ability to execute scripts. And I want to be able to execute any written script from the scripts folder.
I want also the ability to track which scripts where executed so that it can't be executed twice.
Use built in abilities of EF if it's possible or let's build something from the scratch if there is nothing suitable.
One of the important steps is that I want to be able to trigger those scripts executing from the nx and npm commands.

## Let's put some intial test data script with predefined data.
There must be a single user with login "TestUser" created withing this script. Here is the user password hash to use: `100000.vOz/w0Om09alpFHwTlHtng==.o8MBS/VDaq9+BvE1J33VuK4OBlt0EI9Lywxog/e3iTA=`

For this user I want weight and daily calories records to be created for each day starting from '2025-01-01' and ending with todays date.

### The weight calculation
I want to fake some data points of human weight for each day of the specified test period. The minimum weight step will be 0.1
The weight value must be calculated using Sum of Sine Waves algorithm
Combine multiple sine waves with different frequencies to create natural-looking variation:

Long-term wave (full period in days) for seasonal trends
Medium-term wave (period ~30-60 days) for monthly fluctuations
Short-term wave (period ~7-14 days) for weekly variation
Add small random noise for daily fluctuation

**IMPORTANT** I want you to create formula and use it for each required date, do not generate entries by yourself.

### The calories calculation
Now, let's imagine that I want to generate some daily intake of calories for a human for the same period of time as his weights. As you remember we have Morning, Midday, Evening and Night parts of the day.

Algorithm Design
1. Daily Meal Structure (Probabilistic)

Morning: 70-90% chance of eating (1-2 meals typical)
Midday: 85-95% chance of eating (1-3 meals common)
Evening: 80-95% chance of eating (1-2 meals typical)
Night: 5-15% chance of eating (0-1 meals, rare snacking)

2. Meals per Part of Day (Weighted Distribution)
Morning: 70% one meal, 20% two meals, 10% skip
Midday: 60% one meal, 30% two meals, 10% three meals
Evening: 65% one meal, 25% two meals, 10% one large meal
Night: 90% skip, 8% one snack, 2% multiple items

3. Calorie Distribution per Meal
Use realistic ranges based on meal type:

Breakfast: 250-600 calories (normal dist, mean ~400)
Lunch: 400-800 calories (normal dist, mean ~600)
Dinner: 500-900 calories (normal dist, mean ~700)
Snacks: 100-300 calories (normal dist, mean ~200)
Night snacks: 50-250 calories (skewed lower)

4. Food Name Assignment
Create food pools per part of day:
```
foods = {
    'Morning': ['Oatmeal', 'Toast', 'Eggs', 'Yogurt', 'Cereal', 'Pancakes', 'Fruit', 'Coffee & pastry'],
    'Midday': ['Sandwich', 'Salad', 'Pasta', 'Pizza', 'Burger', 'Rice bowl', 'Soup', 'Wrap'],
    'Evening': ['Chicken dinner', 'Fish & vegetables', 'Steak', 'Pasta', 'Stir-fry', 'Curry', 'Tacos'],
    'Night': ['Chips', 'Ice cream', 'Cookies', 'Fruit', 'Nuts', 'Leftovers']
}
```

5. Weekly & Seasonal Patterns

Weekends: +15-25% chance of extra meals, +10% calories per meal
Holidays: Spike days with 2-3x normal intake
Social eating: Random days (5-10% of days) with elevated Midday/Evening calories
Diet periods: Random 2-4 week stretches with -20% calories

6. Correlation with Weight Data
Link to your weight function:

Higher weight periods → slightly more calories (+5-10%)
Weight loss periods → fewer calories (-10-15%)
Add lag effect (calories affect weight 3-7 days later)

**IMPORTANT** I want you to create formula and use it for each required date, do not generate entries by yourself.

## Documentation update
Write an update to the root README file as well as hhmh.instructions.md in order to reflect the latest changes and planned phases. We must retain the previous completed plans as well.