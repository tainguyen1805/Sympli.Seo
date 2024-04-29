# Sympli.Seo

## We need to runs these 2 projects:
- Sympli.Seo.Application.HttpApi
- Sympli.Seo.Application.Web calls the above API

## There are two search providers applied in this app:
- Google: use HttpClient to fetch HTML
- Bing: use selenium ChromeDriver to fetch HTML because bing do not allow fetching html directly via a URL using HttpClient (There're some client redirection to see the result on next page)

##Improvements:
- Applying distributed cache if there's a need to deploy mutiple instances
- Applying Unit testing when I have more time
- Perfomance: Finding out another ways to grab html as Selenium WebDriver is too slow

## Projects dependencies
![image](https://github.com/tainguyen1805/Sympli.Seo/assets/29473882/278089e5-350f-4e14-acd7-33f93715c40e)
