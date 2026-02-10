# Authorization refactoring
Let's change how we deal with authorization

## API changes
I don't want to check user manually on each of the controllers.
Let's introduce some middleware or custom auth attribute or perhaps extend the behavior of default auth attribute to verify user.

If it's possible I don't want to write any bit of authorization endpoints (perhaps except user management).
Suggest how this could be changed using some modern package or library. It seems that correct introduction of OAuth standard could help us a lot.

## UI changes
Similarly to the API side of things I want to use some existing library to manage auth. Ideally it must be implementation of OAuth standard. You are free to suggest something else if you think it is a better choice. Ideally I want to get the token automatic refresh in the background if application is in use.

## Documentation update
Write an update to the root README file as well as hhmh.instructions.md in order to reflect the latest changes and planned phases. We must retain the previous completed plans as well.
