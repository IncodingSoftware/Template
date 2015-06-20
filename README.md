<p style="text-align: left;"><a href="http://blog.incframework.com/wp-content/uploads/2014/01/Article-8_Small_2.png"><img class="aligncenter size-full wp-image-1461" alt="Article-8_Small" src="http://blog.incframework.com/wp-content/uploads/2014/01/Article-8_Small_2.png" width="800" height="300" /></a></p>
<p style="text-align: left;"><b>disclaimer</b>: the article represents a survey on the transformation of JSON data into html on the client part (browser) and reveals the operation details of <strong>template </strong>в <strong>Incoding Framework </strong>(search, formation, local storage and plugging in one’s own engine). Examples from the article are available at <a href="https://github.com/IncodingSoftware/Template">GitHub</a></p>
<p style="text-align: left;"></p>

<h1 style="text-align: center;">Why is the server one not good ?</h1>
Before answering this question, one should study what asp.net mvc can offer.  Razor  - is a server template, which is by default a feature of asp.net mvc, it has a huge functionality and can perform C# code declared within the cshtml  file framework, therefore it does not cause problems, so, what is the matter? When the fed back result for Action  is built up by forming ( View or PartialView )  html  on the server one will get a ready-made content, rather than “pure” data, which may cause the following problems:
<ul>
	<li>One may not use Action  when developing a mobile application or a third-party client (API )</li>
	<li>The traffic used for data transmission grows due to the bulky html content compared to the compact JSON  data.</li>
	<li>The forming takes up the server resources, which can be avoided by delegating them to the client. (they are more numerous, so we don’t mind).</li>
</ul>
<em>Note: as far as the client is concerned, it is of course a joke, that we don’t mind, the actual performance will be dealt with later on in this article. </em>

We may say that content formation on server side has much more opportunities, but possesses a set of problems which can't be solved without turning to client part..<strong>
</strong>
<h1 style="text-align: center;"></h1>
<h1 style="text-align: center;">There is a solution</h1>
КThough  Razor is good, modern applications require the data to be fed back as JSON or xml, therefore server-based html construction does not fit every scenario. On using client pure template engine. A list of requirement was dressed up, that can be implemented in our “wrapper”.
<ul>
<ul>
	<li><strong>Typing</strong></li>
	<li><strong> IML Integration</strong></li>
	<li><strong>Thin template</strong></li>
	<li><strong>Replacement for the “hot” one</strong></li>
	<li><strong><strong>Something of your own</strong></strong></li>
</ul>
</ul>
&nbsp;
<h2 style="text-align: center;"><span style="font-size: 1.5em;">Typing</span></h2>
<h3>Magic string</h3>
Razor’s advantage is that there is intellisense support and instrument for refactor ( Rename, Delete ). Comparing it to the client’s one, let us consider an example in order to see the difference.
<pre>@each(var item in  Model)
{ First Name: @item .FirstName  }</pre>
<em> Note: Server implementation allow to receive a model scheme and subsequently use the known in advance rather than dynamic types. </em>
<pre>{{each data}}
 First Name: {{FirstName}} 	 
{{/data}}</pre>
<address><span style="color: #ff0000;"><em> Note: an example of  template for handlerbars, where ordinary strings are used and Visual Studio can’t calculate in advance the areas included. </em></span></address>FirstName being renamed for Name via special instruments for refactor, such as  R#,  will affect View, but this is not supported by handlerbars since there is no link between the code and template.
To solve this task a builder  has been made up for snap-to for the selected engine ( handlebars,mustaches etc. ) .
<pre class="lang:c# decode:true">using (var template = Html.Incoding().ScriptTemplate&lt;Model&gt;(tmplId))
{
   using (var each = template.ForEach())
   { First Name: @each.For(r= &gt;r.FirstName)  }
}</pre>
&nbsp;

[wpspoiler name="ScriptTemplate vs Template" ]
Incoding Framework has two ways of creating  template
<ul>
	<li><strong>ScriptTemplate</strong> -  the resulting markup is placed in <b>script</b>  tag with a preset Id</li>
</ul>
<pre class="lang:c# decode:true">&lt;script id="templateId" type="text/x-mustache-tmpl"&gt;
   {{#data}}
       &lt;option {{#Selected}}selected="selected"{{/Selected}} value="{{Value}}"&gt;
        {{Text}}
       &lt;/option&gt;
   {{/data}}
&lt;/script&gt;</pre>
<em> note: any type should be indicated as <b>type</b> except for <b>javascript</b>, in order for the browser not to perform the embedded code</em>
<ul>
	<li><strong>Template - </strong> “pure” resulting mark-up.</li>
</ul>
<em>Note: this ways is useful when <b>template</b>  feeds back as a result from  <b>Action </b>( a detailed review will be represented in the  integration with IML  block)</em>

[/wpspoiler]
<h3>Syntax</h3>
The ITemplateSyntax includes methods used for building up  template, in order to register it an inscription should be added to IoC ( Bootstarpper.cs )
<pre class="lang:c# decode:true">registry.For&lt;ITemplateFactory&gt;().Singleton().Use&lt;TemplateHandlebarsFactory&gt;();</pre>
<em>Note: Handlebars ( default by nuget  ) and  Mustaches  implementations are currently set, but individual TemplateSyntax may b written basing on their example. . </em>
<ul>
	<li><strong>ForEach</strong>  - is a cycle on collection ( razor –based counterpart of @foreach(var item in Model) {} )
<ul>
	<li>On major (data received in response )</li>
</ul>
</li>
</ul>
<pre>using(var each = template.ForEach()) { //some template }</pre>
<ul>
<ul>
	<li>On embedded</li>
</ul>
</ul>
<pre class="lang:c# decode:true">@using (var innerEach = each.ForEach(r = &gt; r.Itmes))
{ // some template }</pre>
<ul>
	<li><strong>NotEach -</strong>displays the content if there is no data ( razor-based counterpart of  @if(Model.Count == 0) { } )</li>
</ul>
<pre class="lang:c# decode:true">using (var each = template.NotEach()){ // some template }</pre>
<em>Note: can be used together with ForEach</em>
<pre class="lang:c# decode:true">&lt;ul&gt;
  @using (var each = template.ForEach())
  {  &lt;li&gt;@each.For(r = &gt; r.Title) / @each.For(r = &gt; r.Code)&lt;/li&gt;  }
  @using (var each = template.NotEach())
  { &lt;li&gt; No data &lt;/li&gt; }
&lt;/ul&gt;</pre>
<ul>
	<li><b>For </b> - displays the content of the indicated field (razor-based counterpart of   Model.Title )</li>
</ul>
<pre class="lang:c# decode:true">each.For(r = &gt; r.Title)</pre>
<ul>
	<li><strong> ForRaw</strong> - displays the content of the indicated field without coding  HTML ( razor-based analogue of Html.Raw(Model.Title) )</li>
</ul>
<pre class="lang:c# decode:true">each.ForRaw(r = &gt; r.Title)</pre>
<ul>
	<li><strong>Is</strong> - means to display, if the field is  True or  NOT NULL ( razor-based counterpart of @if(Is != null || Is){} )</li>
</ul>
<pre class="lang:c# decode:true">using(each.Is(r = &gt; r.Property)) { // some template }</pre>
<ul>
	<li><strong>Not</strong> - means to display, if the field is   False or NULL (razor-based counterpart of  @if(Is == null || !Is) {} )</li>
</ul>
<pre class="lang:c# decode:true">using(each.Not(r = &gt; r.Property)) { // some template }</pre>
<ul>
	<li><strong>Inline</strong> - depending on the field value displays the true content ( True or NOT NULL ) or false content ( False or NULL ) ( razor –based counterpart of @(Is ? “true-class” : “false-class”) )</li>
</ul>
<pre class="lang:c# decode:true">@each.Inline(r = &gt; r.Is, isTrue: "true-class", isFalse: "false-class")</pre>
<em>Note:  Is and Not are used to build big blocks, while  Inline draws back the content at once. </em>
<em>Note: Inline has counterpart IsInline, NotInLine, that only show one part of the condition. </em>

[wpspoiler name="Sample" ]
<ul>
<ul>
	<li>Set class as red if true</li>
</ul>
</ul>
<pre class="lang:c# decode:true">&lt;span class="@each.IsInline(r= &gt;r.Is,"red")"&gt;&lt;/span&gt;</pre>
<ul>
<ul>
	<li>  Hide element if true or show overwise</li>
</ul>
</ul>
<pre class="lang:c# decode:true">&lt;span style="@each.Inline(r= &gt;r.Is,isTrue:"display:block",isFalse:"display:none")"&gt;&lt;/span&gt;</pre>
<ul>
<ul>
	<li> Display title if true</li>
</ul>
</ul>
<pre class="lang:c# decode:true">using (each.Is(r = &gt; r.Is))
{ &lt;h3&gt;Header&lt;/h3&gt; }</pre>
<pre class="lang:c# decode:true">@each.IsInline(r= &gt;r.Is,@&lt;h3&gt;Header&lt;/h3&gt;)</pre>
[/wpspoiler]

&nbsp;
<ul>
	<li><strong>Up</strong> -  to go up the hierarchy</li>
</ul>
<pre class="lang:c# decode:true">each.Up().For(r = &gt; r.Property)</pre>
<em>Note: the method is necessary when it is necessary to get the value (condition) of the field, but within the domestic ForEach </em>
<pre class="lang:c# decode:true">using (var each = template.ForEach())
{
   using (var innerEach = each.ForEach(r = &gt; r.Items))
   { <span> @each.Up().For(r= &gt;r.ParentProperty) </span>  }
}</pre>
<ul>
	<li><span style="font-size: 1.5em;">Summary</span></li>
</ul>
<pre class="lang:c# decode:true">@using (var template = Html.Incoding().Template&lt;ComplexVm&gt;())
{
  using (var each = template.ForEach())
  {
    &lt;div&gt; 
     &lt;ul style="@each.IsInline(r = &gt; r.IsRed, "color:red;")"&gt;
        @using (var countryEach = each.ForEach(r = &gt; r.Country))
        {
         &lt;li&gt;
            @using (each.Up().Is(r = &gt; r.IsRed))
            { &lt;span&gt;Country @countryEach.For(r = &gt; r.Title) from red group @each.Up().For(r = &gt; r.Group)&lt;/span&gt; }	 	 
            @using (each.Up().Not(r = &gt; r.IsRed))	 	 
            { &lt;span&gt;Country @countryEach.For(r = &gt; r.Title) by group @each.Up().For(r = &gt; r.Group)&lt;/span&gt; }	 	 
           &lt;dl&gt;	 	 
             @using (var cityEach = countryEach.ForEach(r = &gt; r.Cities))	 	 
             { &lt;dd&gt; City: @cityEach.For(r = &gt; r.Name) &lt;/dd&gt; }	 	 
           &lt;/dl&gt;	 	 
         &lt;/li&gt;	 	 
        } 	 	 
    &lt;/ul&gt;	 	 
 &lt;/div&gt;	 
 } 
}</pre>
<em> note: downloading the example is available on  <a href="https://github.com/IncodingSoftware/Template/blob/master/Template.UI/Views/Template/Complex_Tmpl.cshtml">GitHub</a></em>

&nbsp;
<h2 style="text-align: center;">integration with IML</h2>
<pre class="lang:c# decode:true">@(Html.When(JqueryBind.InitIncoding)
      .Do()
      .AjaxGet(Url.Action("FetchCountries", "Data"))
      .OnSuccess(dsl = &gt; dsl.Self().Core().Insert.WithTemplate(tmplId.ToId()).Html())
      .AsHtmlAttributes()
      .ToDiv())</pre>
IML has methods (AjaxGet, Submit, AjaxPost), to get the data, which can further be inserted via the Insert. The data can be html content or json objects. In order to insert json objects template is used, the path to which is specified by Selector.

<em>NOTE: Starting with version 1.2, methods or WithTemplateById WithTemplateByUrl are preferable to use. </em>
<ul>
	<li><strong>WithTemplateById</strong> -to find a dom element on Id, which contains  template</li>
</ul>
<pre class="lang:c# decode:true">dsl.Self().Core().Insert.WithTemplateById(tmplId) // by Selector.Jquery.Id(tmplId)</pre>
<em>NB: to build template ScriptTemplate(tmplId)should be used in this case. </em>

&nbsp;

[wpspoiler name="sample" ]
<pre class="lang:c# decode:true">@{
    string tmplId = Guid.NewGuid().ToString();
    using (var template = Html.Incoding().ScriptTemplate&lt;CountryVm&gt;(tmplId))
    {
        &lt;ul&gt;
            @using (var each = template.ForEach())
            { &lt;li&gt;@each.For(r =&gt; r.Title) / @each.For(r =&gt; r.Code)&lt;/li&gt; }
        &lt;/ul&gt;
    }
}
@(Html.When(JqueryBind.InitIncoding)
      .Do()
      .AjaxGet(Url.Action("FetchCountries", "Data"))
      .OnSuccess(dsl =&gt; dsl.Self().Core().Insert.WithTemplateById(tmplId).Html())
      .AsHtmlAttributes()
      .ToDiv())</pre>
[/wpspoiler]
<ul>
	<li><strong>WithTemplateByUrl</strong> - Download layout on ajax</li>
</ul>
<pre>dsl.Self().Core().Insert.WithTemplateByUrl(url) // by Selector.Incoding.AjaxGet(url)</pre>
<h4 style="text-align: left;"></h4>
<span style="font-size: 13px;">[wpspoiler name="Sample" ]</span>
<h5>Controller</h5>
<pre class="lang:c# decode:true">public ActionResult Template()
{
    return IncView();
}</pre>
<h5>View Template</h5>
<pre class="lang:c# decode:true">@using (var template = Html.Incoding().Template&lt;AgencyModel&gt;())
{
    using (var each = template.ForEach())
{ &lt;span&gt; @each.For(r = &gt; r.Name)&lt;/span&gt; }
}</pre>
<i> N.B.: when building up template for ajax Template rather than ScriptTemplate method should be used. </i>
<h5>View IML</h5>
<pre class="lang:c# decode:true">Html.When(JqueryBind.InitIncoding)
    .Do()
    .AjaxGet(Url.Action("GetAgencies", "IncAgency"}))
    .OnSuccess(dsl = &gt; dsl.Self().Core().Insert.WithTemplate(Url.Action("Template", "IncAgency").ToUrl()).Append(); })
    .AsHtmlAttributes()
    .ToDiv()</pre>
<strong>Does each template has its own Action ?</strong>

There are two solutions that allow not to duplicate the code in <b>action</b>:
<ul>
	<li><strong>Shared action</strong></li>
</ul>
<pre class="lang:c# decode:true">public class SharedController : IncControllerBase
{
   public ActionResult Template(string path)
   {
      return IncView(path);
   }
}</pre>
<em>Note: extensions for Url can be built, in order to add the path check via ReSharper annotation </em>
<pre class="lang:c# decode:true">public static class UrlExtensions
{
    public static string Template(this UrlHelper urlHelper, [PathReference] string path)
    {
        return urlHelper.Action("Template", "Shared", new { path = path });
    }
}</pre>
<ul>
	<li><a title="MVD ( Model View Dispatcher )" href="http://blog.incframework.com/model-view-dispatcher/"><strong>MVD</strong></a></li>
</ul>
<pre class="lang:c# decode:true">Url.Dispatcher().AsView("path to template")</pre>
<em> Note: MVD was initially positioned as a universal <b>template</b> loader</em>

[/wpspoiler]
<h4>Id vs Url</h4>
Initially, we used a dom element (script) as storage template layout, but gradually switched to boot with ajax, which has the following advantages:
<ul>
	<li>Re-use template on different View</li>
</ul>
<em>Note: for Id it was realized bringing out template into Layout (another master page) </em>
<ul>
	<li>Layout is factored out of View</li>
</ul>
<em>Note: for Id it was realized through partial view </em>
<ul>
	<li>Lazy load (template is loaded on demand)</li>
</ul>
<em>Note: especially true when using tabs</em>
<h2 style="text-align: center;">thin template</h2>
We chose engine, which support logic less template, such as Mustaches, Handlerbars, because this approach allows to simplify View, moving complex logic on the server, where it is easier "to deal with the complexity."

For clarity, a task that will be solved in the "normal" way and using logic less
<ul>
	<li><strong>Ordinary</strong></li>
</ul>
<pre class="lang:c# decode:true">if(Model.Count &lt;= 5 &amp;&amp; Model.Type == TypeOf.Product)
{//something code }</pre>
<ul>
	<li><strong>Logic Less</strong></li>
</ul>
<pre>if(Model.IsLimitProduct) // public bool IsLimitProduct { get {return Count &lt;= 5 &amp;&amp; TypeOf == Product }}
{ //something code }</pre>
In the first case, we calculate the value in View, but in logic less we calculate the expression beforehand on the server that has the following advantages:
<ul>
	<li>Re-use in other scenarios</li>
	<li>Unit Test can be covered</li>
	<li>Less code into View layout</li>
</ul>
ПAdvantages of logic less manifest when increasing complexity of the tasks, because to expand and maintain the conditions easier on the server side than in View
<h2 style="text-align: center;">Replacement for the "hot""</h2>
Task appeared, after problems had been discovered with mustaches, which slowed down in ie 8 and below, and also had problems with inserting large amounts of data (more than 30 entries). Since the implementation of mustaches used in a number of projects, the choice of engine should be simple, so that any could be used.

<em>Note: the code, described below, is available at <a href="https://github.com/IncodingSoftware/Template/blob/master/Template.UI/Views/Home/Custom_Engine.cshtml">GitHub</a></em>
<h5>JavaScript Code</h5>
<pre class="lang:c# decode:true">function IncJqueryTmplTemplate() {
    this.compile = function(tmpl) {
        return tmpl;
    };
    this.render = function(tmpl, data) {
        var container = $('&lt;div&gt;');
        $.tmpl(tmpl, data).appendTo(container);
        return container.html();
    };
}</pre>
<em>  Note: Since not all engine support pre compile, tmpl can be returned unchanged</em>
<h5>Layout Code</h5>
<pre class="lang:c# decode:true">&lt;script type="text/javascript"&gt;
    ExecutableInsert.Template = new IncJqueryTmplTemplate();
&lt;/script&gt;</pre>
<em> Note: The code is in layout, because it must be before the first call Insert.WithTemplate</em>
<h5>Template Code</h5>
<pre class="lang:c# decode:true">@{ string tmplId = Guid.NewGuid().ToString(); }
&lt;script type="jquery-tmpl" id="@tmplId"&gt;
    {{each data}}
      &lt;li&gt;${Title}&lt;/li&gt;
    {{/each}}
&lt;/script&gt;</pre>
<em> note: template is built on a "pure" jquery tmpl, without using ITemplateSyntax , but the implementation can be written and registered in IoC</em>
<h5>IMl Code</h5>
<pre class="lang:c# decode:true">@(Html.When(JqueryBind.InitIncoding)
      .Do()
      .AjaxGet(Url.Action("FetchCountries", "Data"))
      .OnSuccess(dsl =&gt; dsl.Self().Core().Insert.WithTemplateById(tmplId).Html())
      .AsHtmlAttributes()
      .ToTag(HtmlTag.Ul))</pre>
<em>  Note: from IML, nothing has changed, so template engine can be easily replaced without significant alterations </em>

&nbsp;
<h2 style="text-align: center;">Something of one’s own</h2>
<h5>Faster, much faster !!</h5>
For the first versions of framework, we stored template in dom elements (script), but this method did not allow to build lazy load, so then we switched to ajax loading, which had other problems:
<ul>
	<li>If many elements are loaded immediately on one home page that the pool of browser requests is filled up</li>
</ul>
<em>note: this is partly solved by using cache, but the request will still be, though the status is 304 </em>
<ul>
	<li>Absence of pre-compile for engine</li>
</ul>
<em>Note: especially true when there is a large number of inserted data (more than 30 objects)</em>
<h5>Once and forever</h5>
The solution was found in Local Storage, which allows to save template in browser and then to use it all the time. Will there always be one template? – To answer this question, let’s look at the code (pseudocode) of this mechanism operation.
<pre class="lang:c# decode:true">if(storage.Contain(selector.ToKey() + TemplateFactory.Version)
  return storage.Get(selector.ToKey() + TemplateFactory.Version);

var template = selector.Execute();
storage.Add(template.PreCompile());
return storage.Get(selector.ToKey() + TemplateFactory.Version);</pre>
<ul style="list-style-type: square;">
	<li><strong>Line 1</strong> - check availability template in local storage</li>
</ul>
<em>Note: selector and version are keys (details about version below)</em>
<ul style="list-style-type: square;">
	<li><strong>Line 2</strong> -  return  template from local storage</li>
	<li><strong>Line 4</strong> - obtain selector value</li>
	<li><strong>Line 5</strong> - add template to local storage</li>
</ul>
<em>Note: before adding pre compile should be done</em>
<ul style="list-style-type: square;">
	<li><strong>Line 6</strong> -return   template from local storage</li>
</ul>
And if I change the layout template, but the key will remain the same? - In order to solve this problem, we have added versioning globally. To specify the current version of all template field TemplateFactory.Version must be set using JavaScript
<pre class="lang:c# decode:true">&lt;script&gt;
TemplateFactory.Version = '@Guid.NewGuid().ToString()';
&lt;/script&gt;</pre>
Note: the code should be carried out before calling Insert.WithTemplate, so it is best to be placed in Layout
On the example I set Guid, which guarantees the new version after the full (F5) page reloading, but this behavior is relevant only for the development process, and when the code will be laid out in production, it is necessary to fix the version.

[wpspoiler name="Our policy versioning" ]
<ul>
	<li><strong>Debug    </strong>- while  developping the mark up activity rate  in the template is high, so the version is updated as often as possible (we use Guid, to maintain its uniqueness)</li>
	<li><strong><strong>Release - </strong></strong><strong> </strong>after the project has been uploaded on the server, a fixed version must be installed</li>
</ul>
<h5>Layout</h5>
<pre class="lang:c# decode:true">&lt;script type="text/javascript"&gt;
TemplateFactory.Version = '@CurrentSettings.CurrentVersion';
&lt;/script&gt;</pre>
<h5>Current Version</h5>
<pre class="lang:c# decode:true">public string CurrentVersion
{
   get
   {
#if DEBUG
    return Guid.NewGuid().ToString();
#else
    return Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version.ToString();
#endif
   }
}</pre>
<em> note: <strong>TeamCity</strong> <b><b><a href="http://confluence.jetbrains.com/display/TCD7/Predefined+Build+Parameters">build.number</a> </b></b></em><em><b>used as a fixed version. Our approach to the</b></em><em> continuous integration ( TeamCity, rake )</em><em> will be thoroughly considered in the subsequent papers. <b>  </b> </em>

&nbsp;

[/wpspoiler]
<h3>One for all and all for one</h3>
When building template, it is necessary to specify only the model type, but it is not considered if it is a collection or a single object. Some template engine require to specify what it will be - a collection or a single object (for example handlebars {{# with data}} or {{# each autors}}), so we decided to remove this condition and not to take into account the amount of items when template constructing. Owing to this feature we did not need to add method With (which not all engine support).

[wpspoiler name="пример" ]
<h5>Коллекция объектов</h5>
<pre class="lang:c# decode:true">[HttpGet]
public ActionResult FetchCountries()
{
    return IncJson(GetCountries());
}</pre>
<h5> Один объект</h5>
<pre class="lang:c# decode:true">[HttpGet]
public ActionResult FetchCountry()
{
    return IncJson(GetCountries().FirstOrDefault());
}</pre>
<h5>Общий template</h5>
<pre class="lang:c# decode:true">@using (var template = Html.Incoding().Template&lt;CountryVm&gt;())
{
 &lt;ul&gt;
@using (var each = template.ForEach())
{ &lt;li&gt;@each.For(r = &gt; r.Title) / @each.For(r = &gt; r.Code)&lt;/li&gt; }
 &lt;/ul&gt;
}</pre>
[/wpspoiler]

&nbsp;
<h2 style="text-align: center;">Client is not a server or Our hook (Repeated mistakes)</h2>
<p style="text-align: left;">During training employees to use client template, I compiled a list of the most frequent mistakes that because they write code, as well as for the server template</p>
<strong>GUID</strong> -in view it is used to specify a unique name for the element within the pages, but in the case of the client template it has pitfalls.

Condition: each tag li must have a unique Id
<pre class="lang:c# decode:true">@using (var each = template.ForEach())
{
    var uniqueId = Guid.NewGuid().ToString();
    &lt;li id="@uniqueId"&gt;&lt;/li&gt;
}</pre>
Code contains an error, because the variable uniqueId will be calculated once on the server, bypass of the collection will take place on client. To test this, we must see the resulting layout.
<pre class="lang:c# decode:true">{{each data}}
&lt;li id="EAAF08F8-0F09-4DA7-BE64-6D0075749D76"&gt; &lt;/li&gt;
{{/each data}}</pre>
For the code to work correctly, the logic of guid calculation has to be transfered to Model
<pre class="lang:c# decode:true">@using (var each = template.ForEach())
{
    &lt;li id="@each.For(r= &gt; r.UniqueId"&gt;&lt;/li&gt; // UniqueId {get {return Guid.NewGuid().ToString(); }}
}</pre>
<h2 style="text-align: center;"> Conclusion</h2>
<p style="text-align: left;">We constantly develop each Incoding Framework component, contributing new features to it with every version and "polish" the old ones and certainly template is not an exception, to make sure one can see our BugTracker. All I have described in the article are proven by personal experience constructions and practices that allow to develop cross-platform applications initially, but not to reconstruct the architecture when it costs a lot.</p>
