# API refactoring
Let's change how we structure our API code

## API changes
Bring the correct layered architecture.
Use services for the data processing and repository pattern for the database operations.
No business logic code in the controllers please.
Entities must be created using factories, no internal private methods for this purpose.
Let's extract service registration to a separate extension class.

Check all existing API classes and apply best programming practices available.

## Documentation update
Write an update to the root README file as well as hhmh.instructions.md in order to reflect the latest changes and planned phases. We must retain the previous completed plans as well.
