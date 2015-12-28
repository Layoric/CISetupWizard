# TeamCityClient
TeamCity v9 client using ServiceStack.Client with demo AppHost, still a work in progress.

### AMI Setup
1. Install TeamCity
2. Install [TeamCity.Node](https://github.com/jonnyzzz/TeamCity.Node) plugin
3. Add Default NuGet verison in TeamCity admin.
4. Add IIS + ASP.NET Role
5. Use Web Platform Installer to Add Web Deploy
6. Enable Web Deploy through IIS
7. Install NodeJS + NPM
8. npm install bower -g 
9. Install Git (v1.9.5 as v2.6.3 currently bugged causing "No working directory found")
10. Add `MSDeployPath` Environment variable
11. Create `CIWizard` user, add to `Administrators` group, disable RDP -> For IIS Management + CIWizard AppPool Account
12. Create `wizard_deploy` 'IIS Manager' user.
![](https://github.com/Layoric/TeamCityClient/raw/master/images/iis-manage.png)
13. Add `ss.msdeploy.username` and `ss.msdeploy.password` as `wizard_deploy` credentials
14. Deploy CIWizard app locally, remembering to assign `CIWizard` user to app pool.

Lastly, edit `config.xml` of Amazon settings, update `EC2SetPassword` to `Enabled` and then stop EC2 instance and create AMI.


