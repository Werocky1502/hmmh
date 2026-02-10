---
Let's change app title in index.html as well as favicon.
The desired default title is "Help me manage health app".
Favicon must be some svg representing heart with pulse line.
---
Let's change the heart color to be in green tones to match the slight gradient of the current app.
---
Let's update the root page.
I want to add some warning text similar to the notification popup at the right bottom color of the page. The text must be "Warning! Almost zero human intelligence was used during application programming!"
(please rephrase it a bit if you think there are grammar issues there.
I also don't mind to hear from you a couple of other funny text options to put there)
---
I like "Warning: this app was built with a suspiciously low amount of human intelligence.".
But it doesn't look like a warning in terms of color. Also could we make it floating a bit to drag attention
---
Great! Now I want to make it wider so that more text fit into one line.
The speed and distance of floating are perfect, let's keep it.
Also let's make it float not only up and down but left and right as well.
---
Let's proceed with changes to the dashboard.
I want header of each dashboard widget to be centered
---
I want current date to be printed at the left side of the global date filtering row. It must be changed on every page where global date filter exist.

The weights chart dashboard widget must be updated to not show date range but to show average weight instead with measuring unit.

Calories chart dashboard widget must also have measuring unit added near the avg value.

My weight today widget must no longer have date in the header. The input and button must be centered inside the widget.

Log calories today widget must no longer have date in the header. The inputs must be centered and the save button to be placed at the very bottom.
---
Let's get rid of header badges in the chart widgets. Represented info must be moved to the bottom section of the chart widget right to the min/max value and before entries.

Let's allow my weight today widget to has smaller height to fit the input form.

Let's turn notes input into a text input instead of area and let's place it to the right after part of the day input
---
Let's put the input and button in the my weight today widget to the very center of the widget.

Let's make sure that inputs in the log calories today are of the same size
---
Let's move "Save weight" button to a new line
---
Let's move the save weight button to the very bottom and at center of the widget.
---
Let's move to the /weights page. Let's replace Range info under the chart to be average instead (similar to the average built in the weight widget on dashboard)
---
Let's move to the /calories page. Let's replace Range with average like we did on /weights page. It must be based on the total sum and not by day part.

Also let's change wording for the 'by part' badge. Let it be 'by day part'

Let's make the note field to be input (like we did on the dashboard) and put it between date and part of the day. Save button must be on a separate row under the input fields
---
Let's modify each of the pages to change the html title (if there is a popular library for this purpose let's use it) (if it's easier to do with custom hook then let's not introduce the library).

The title per page are following:
/dashboard - HMMH (Dashboard)
/weights - HMMH (Weights)
/calories - HMMH (Calories)
---