# UI refactoring
Let's change how we structure our UI code

## UI changes
Carefully split pages into reusable components.
Where it is possible use services for data preparation and business logic calculations.
It's very important to strictly stick to the one-way data flow, but avoid props drilling hell.
I want pages to be as modular as possible.
Think hard on chart component abstraction so that we have a single chart component with all the necessary props passed from the outside of the component.
We might also want to create some generic components for the widgets and common data filters.
The pages template could also be abstracted away.
We don't want code duplication unless it is absolutely necessary.

## Documentation update
Write an update to the root README file as well as hhmh.instructions.md in order to reflect the latest changes and planned phases. We must retain the previous completed plans as well.
