SchedulesDirect.Json
====================

Library for getting TV guide data from Schedule Direct's JSON web API

## Nuget
This library is available in Nuget as [SchedulesDirect.Json](http://nuget.org/packages/SchedulesDirect.Json/)
## Unit Tests
To run the unit tests, you'll need to put a valid username and password in the app.config file of the UnitTests project.

**Warning:** the unit tests *will* use up your maximum number of headend changes.

##Issues with the API 
(hoping Schedules Direct will fix)

* Separate requestJSON.php that accepts plain JSON instead of form encoded
* Include IDs in JSON instead of having to parse from filenames in zip
* Request for headends is trying to send as a file
* Error thrown for empty headends
* atscMajor, atscMinor, and uhfVhf should be in the channel map instead of the station data
* Metadata get isn't valid JSON.  Should be valid or should be multiple files in zip like other commands.
