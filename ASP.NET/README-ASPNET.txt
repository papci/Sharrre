A TINY TUTORIAL 
----------------

NB : You can use Sharrre.ashx in any ASP.NET applications (MVC or WebPages)

ASP.NET Part
--------------

1) Create a folder "Handlers" in your project (but you can name it whatever you want)
2) Add Sharrre.ashx in this folder 
3) If you're using MVC 4, add routes.IgnoreRoute("{resource}.ashx/{*pathInfo}"); in your RouteConfig.cs
4) You'll need two NuGet packages : HtmlAgilityPack and JSON.NET (JSON.NET is now included in all ASP.NET MVC 4 applications)


jQuery Configuration Part
-------------------------

1) Insert your share JS files as usual
2) Specify  "curlUrl". Ex :
$('#share').sharrre({
  share: {
    googlePlus: true,
    facebook: true,
    twitter: true
  },
  url: 'http://sharrre.com/',
  urlCurl: '/Handlers/Sharrre.ashx'
});

3) Have a coffee


