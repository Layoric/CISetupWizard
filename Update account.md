## Change Default password
The CI Setup Wizard interacts with TeamCity via an authenticated TeamCity user account. This account is setup with a default username and password, `administrator` / `$ChangeMe$`.

After the initial setup, this password should be changed in both TeamCity and the CI Setup Wizard config.

1. Change TeamCity `administrator` user password by using the TeamCity web interface, navigating to `Administration -> Users -> administrator.
2. Update the CI Setup Wizard `appsettings.txt` file with the new password by opening `C:\inetpub\wwwroot\cisetupwizard\appsettings.txt`.

> If you'd like to separate the CISetupWizard user from the `administrator` account, create another TeamCity account with administrative privileges and update both `UserName` and `Password` application settings in CI Setup Wizard to match.