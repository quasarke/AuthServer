# initial setup

## initialize the database
set the connection string in the appsettings to the desired database.

read through the commented code and select providers you would like to configure

to initialize the database go to the startup.cs uncomment the //InitializeDatabase(app); method in the configurer method run the application the tables will be generated automaticly on startup

be sure to comment out the initialize function after the first run otherwise you'll wipe out the database in subsequent runs.

## setup email

The email settings are in the appsettings the email templates exist in the EmailTemplates folder in root. the various links are to views that might be used in an email.

```
  "ElasticEmailConfig": {
    "smtpHost": "smtp.mymailserver.biz",
    "smtpUser": "postmaster@company.com",
    "smtpPassword": "*****",
    "messageFrom": "membership@vehicletracking.com",
    "messageFromDisplay": "Member Services", //the name that will be display on the email
    "VerifyEmailTemplate": "verifyemail.html", //the template used to verify a persons email
    "UserNameTemplate": "username.html", // the username template for a forgot username request
    "PasswordResetTemplate": "passwordreset.html", // the forgot password reset template
    "VerifyEmailWhyUrl": "https://login.company.com/why", // for why your receiving this
    "VerifyEmailUrl": "https://login.company.com/Register", // register new user link
    "PasswordResetUrl": "https://login.company.com/ForgotPassword" // forgot password page
  }
```
