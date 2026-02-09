# Weight management
Let's define user weight data structures, UI forms and related APIs.

## Weight management API
We need to store a weight of a user in kilograms.
The only supported weight tracking for the user is once per day.
We want to store date only, weight, user reference and unique id.
Let's create a separate database table for this matter.
Let's create a separate API endpoints to put weight, to get a single weight record, to get all records for the certain date range.

## Weight management UI
The main dashboard page need to have global date filtering with two date pickers. Those date pickers must influence the whole page data filtering. By default there will be a one week range selected starting from today and week backward. So upon the page loading we want to retrieve all weight records for the specified filters. We want to display this data as a dashboard weight chart. We also want to calculate min and max weight for the specified dates and show the results under the chart in the same block.

There must be a separate section under the charts. This section heading must be "My weight today". The section must contain a single input field allowing to put the weight into it and a button to save the changes to the API. If there is a weight already recorded for today, we must display it in the input immediately.

Let's create a separate "/weights" page which will have the same date filtering but defaulted to once month period. There must be the same "My weight today section" as on dashboard page. Also there must be a table with weights and dates for the filtered period.

## Documentation update
Write an update to the root README file as well as hhmh.instructions.md in order to reflect the latest changes and planned phases. We must retain the previous completed plans as well.
