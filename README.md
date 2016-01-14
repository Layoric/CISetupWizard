# CISetupWizard
An application to simplify the creation of continuous deployment projects in TeamCity.

### Change Default password
The CI Setup Wizard interacts with TeamCity via an authenticated TeamCity user account. This account is setup with a default username and password, `administrator` / `$ChangeMe$`.

After the initial setup, this password should be changed in both TeamCity and the CI Setup Wizard config.

1. Change TeamCity `administrator` user password by using the TeamCity web interface, navigating to `Administration -> Users -> administrator.
2. Update the CI Setup Wizard `appsettings.txt` file with the new password by opening `C:\inetpub\wwwroot\cisetupwizard\appsettings.txt`.

> If you'd like to separate the CISetupWizard user from the `administrator` account, create another TeamCity account with administrative privileges and update both `UserName` and `Password` application settings in CI Setup Wizard to match.

### AMI Setup
1. Install TeamCity
2. Install [TeamCity.Node](https://github.com/jonnyzzz/TeamCity.Node) plugin (specifically 1.0.58)
3. Add Default NuGet verison in TeamCity admin.
4. Add IIS + ASP.NET Role
5. Use Web Platform Installer to Add Web Deploy
6. Enable Web Deploy through IIS
7. Install NodeJS + NPM
8. npm install bower -g 
9. Make bower available on the path
10. Install Git (v1.9.5 as v2.6.3 currently bugged causing "No working directory found")
11. Add `MSDeployPath` Environment variable
12. Create `CISetupWizard` user, add to `Administrators` group, disable RDP -> For IIS Management + CIWizard AppPool Account
![](https://github.com/Layoric/TeamCityClient/raw/master/images/iis-manage.png)
14. Add `ss.msdeploy.username` and `ss.msdeploy.password` as `CISetupWizard` credentials to `_Root` TC project.
15. Deploy CISetupWizard app locally, remembering to assign `CISetupWizard` user to app pool. (Match name of app to repo so the app can create it's own TC project to iterate locally)

Lastly, edit `config.xml` of Amazon settings, update `EC2SetPassword` to `Enabled` and then stop EC2 instance and create AMI.


