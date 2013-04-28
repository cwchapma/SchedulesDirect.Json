SchedulesDirect.Json
====================

Library for getting TV guide data from Schedule Directs JSON web API

Issues with the API (hoping Schedules Direct will fix)

- Include IDs in JSON instead of having to parse from filenames in zip
- Request for headends is trying to send as a file.
- Error thrown for empty headends
- atscMajor, atscMinor, and uhfVhf should be in the channel map instead of the station data
