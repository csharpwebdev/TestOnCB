# TestOnCB

- Modify DefaultConnection setting value in the appsettings.json file to connect to the right database
- Run update-database to migrate data
- You can see the past results without login
- You can play only when you are logged in

- There was some issue around GetUserAysnc function because of JWT authentication and Oicd, it does not return the currently logged user at all.
So I had to find another way to pass that obstacle
