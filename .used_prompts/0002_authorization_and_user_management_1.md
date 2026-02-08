# Authorization and user management
Here are some of the basic requirements for the authorization and user management in the HMMH application. We will plan and execute next phases based on the following.

## UI Pages
Here is the complete list of the UI pages for this phase. No other pages must exist in the project.
* "/"" - the root page. It is accessible without any authorization. The page has some patterned background and big application name written in the middle of the page. The name is full one, not HMMH shortening. Right under the header there is a single centered button placed. It is either "Log in to start usage" or "Show me the dashboard" depending on whether the user is authenticated.
* "/dashboard" - the main application dashboard. It is not accessible if user is not authorized and we want to redirect user to the authorization page if he tries to get direct access. At the first iteration I want to see some default header on the page with application name in the middle and user login/fake avatar at the right side. The whole user login block is a dropdown with "Logout" and "Delete account" buttons. The main dashboard content will be a fake graph of weight and daily calories/wight stats. Nothing else is needed for this phase.
* "/login" - main authorization page. This page contains some standard Login/Password form. There must be a small link present somewhere near the sign in button to switch the whole form into sign up variant. The action button will change it's action based on whether we doing sign in or sign up. The buttons here just simply calling our .Net api for user management.

## API endpoints
We need to choose authorization technology (suggest a couple of options here, we need something modern protected and free, it also must not involve any third party calls) and implement endpoints for sign in and sign up actions as well as account deletion endpoint. We don't really want to support reach user object. We will be operating only user login/password on front end. All the users must be stored in our database so we need to make sure we have a connection to it and all required data structures are created there. No emails required. Take into considerations that ideally we want selected authorization technology to be easily integrated into UI React app.

## UI and API integration
UI application must be configured in order to use designed API endpoints. The actual API address must be configurable.

## Documentation update
Write an update to the root README file as well as hhmh.instructions.md in order to reflect the latest changes and planned phases. We must retain the previous completed plans as well.
