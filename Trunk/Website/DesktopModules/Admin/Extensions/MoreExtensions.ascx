<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Extensions.MoreExtensions" CodeFile="MoreExtensions.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>  
<div id="dnnAppGallery" class="dnnForm dnnAppGallery dnnClear">
    <span id="loading" class="dnnAppGalleryLoading">Loading...</span>
    <div class="header"><h1><%=LocalizeString("AppGallery") %></h1></div>

    <div class="dnnAppGalleryTags">
        <h2 class="dnnGallerySubHeading"><%=LocalizeString("TagCloud")%></h2>
        <span class="dnnAppGalleryTagList" id="tag-list"></span>

    </div>
    <div class="dnnAppGalleryListing">
        <fieldset>
            <legend></legend>
            <div class="dnnAppGallerySearch">
                <h2 class="dnnGallerySubHeading"><%=LocalizeString("AppGallerySearchTitle") %></h2>
                <div class="dnnFormItem">
                    <label for="typeDDL">
                        <a href="#" onclick="if (__dnn_SectionMaxMin(this,  'typeDDLHelp')) return false;" class="dnnFormHelp" >
                            <span><%=LocalizeString("TypeLabel") %></span>
                        </a>
                    </label>
                    <div id="typeDDLHelp" class="dnnFormHelpContent dnnClear" style="display:none;">
		                <span><%=LocalizeString("TypeLabel.Help") %></span>
	                </div>
                    <select id="typeDDL">
                        <option value="all">All</option>
                        <option selected="selected" value="module"><%=LocalizeString("AppGalleryModule") %></option>
                        <option value="skin"><%=LocalizeString("AppGallerySkin") %></option>
                    </select>        
                </div>
                <div class="dnnFormItem">
                    <label for="searchText">
                        <a href="#" onclick="if (__dnn_SectionMaxMin(this,  'searchHelp')) return false;" class="dnnFormHelp" >
                            <span><%=LocalizeString("SearchLabel") %></span>
                        </a>
                    </label>
                    <div id="searchHelp" class="dnnFormHelpContent dnnClear" style="display:none;">
		                <span><%=LocalizeString("SearchLabel.Help")%></span>
	                </div>
                    <div class="dnnGallerySearch">
                        <input type="text" id="searchText" title=""<%=LocalizeString("SearchLabel.Help")%>" />        
                        <a href="javascript:void(0);" id="search-reset" class="dnnSecondaryAction"><%=LocalizeString("ClearSearch")%></a>
                        <a href="javascript:void(0);" id="search-go" class="dnnSecondaryAction"><%=LocalizeString("Search")%></a>
                    </div>
                </div>
            </div>            

        </fieldset>
        <div id="extensionDetail">
            <div id="extensionDetailInner"></div>
        </div>
        <h2 class="dnnGallerySubHeading"><%=LocalizeString("Extensions")%></h2>
        <fieldset id="extensionsSummary">
            <legend></legend>
            <div class="sort-options fullWidth-Center">
               <span class="sort-button-off"><a href="javascript:void(0);" id="NameSorter"><%=LocalizeString("NameAZ") %></a></span> 
               <span class="sort-button-off"><a href="javascript:void(0);" id="PriceSorter"><%=LocalizeString("PriceLowHigh") %></a></span>
            </div>
            <div id="searchFilters" class="dnnFormHelpContent dnnClear"></div>
            <span id="extensionList" class="extensionList"></span>
        </fieldset>
    </div>

    <script id="tag-tmpl" type="text/html">
{{if tagName}}
    <a href="javascript:void(0);" style="font-size:${fontSize}%" class="tag" alt="${tagName} (${TagCount})" title="${tagName} (${TagCount})" value="${tagID}">${tagName}</a>&nbsp;
{{/if}}
    </script>
    <script id="eTmpl" type="text/html">
	<div>    
	    <div class="dnnProduct ${catalog.catalogCSS} ext-${extensionID}">    
            <a name="ext-${extensionID}"></a>
            <a href="javascript:void(0);" class="dnnProductImage" onclick="return _gallery.ShowDetails(${extensionID})">
                {{if imageURL}}
                    <img class='dnnProductImage' alt='${extensionName}' src="${imageURL}" />
                {{else}}
                    <img class='dnnProductImage' alt='${extensionName}' src='<%=ResolveUrl("~/images/System-Box-Empty-icon.png")%>' />
                {{/if}}
            </a>

            <a href="javascript:void(0);" class="dnnProductTitle" onclick="return _gallery.ShowDetails(${extensionID})">
            {{if extensionName}}
                ${extensionName}
            {{else}}
                <%=LocalizeString("MoreInformation") %>
            {{/if}}
            </a>

            <span class="dnnProductOwner">
            {{if ownerName}}
                    <%=LocalizeString("By") %> <a href="javascript:void(0)" onclick="_gallery.OwnerFilterGallery('${ownerName}')">${ownerName}</a>
            {{/if}}
            </span>
                        
            {{if MinDnnVersion}}
                ${MinDnnVersion}
            {{else}}
                <%=LocalizeString("VersionNotSpecified") %>
            {{/if}}

		    <span class='dnnProductDescription'>{{html Description}}</span>
            
            <span class='dnnProductPrice'>${_gallery.FormatCurrency(price)}</span>

            
            <div class="dnnProductFooter  ${catalog.cssClass}">
            
                <img class='productTypeImage dnnIcon' alt='${extensionType}'  title='${extensionType}'  src='<%=ResolveUrl("~/images/appGallery_${extensionType}.gif")%>'/>

                {{if license}}
                    <a class="galleryLink  inline" onclick="return Gallery.ShowDetails(${extensionID})">                    
                        <img class='productTypeImage dnnIcon' alt='License for ${extensionName}' title='License Specified' src='<%=ResolveUrl("~/images/license.png")%>' />
                    </a>
                {{/if}}
            
                {{if detailURL}}                
                        <a href="${detailURL}" target="_new"><img class='productTypeImage dnnIcon' alt='${extensionType}' src='<%=ResolveUrl("~/images/appGallery_details.gif")%>' /></a>
                {{/if}}                
                <a class="galleryLink inline" href="${detailURL}&PackageOptionID=0&action=Add" target="_new"><img class='dnnIcon shopping-cart galleryLink' alt='Buy ${extensionName}'  Title='Buy ${extensionName}' src='<%=ResolveUrl("~/images/appGallery_cart.gif")%>' /></a>                
                <a class="galleryLink inline"><img class='dnnIcon deploy galleryLink' alt='Deploy ${extensionName}' Title='Deploy ${extensionName}' src='<%=ResolveUrl("~/images/appGallery_deploy.gif")%>' /></a>
                {{if catalog.url}}                
                    <a class="galleryLink inline" href="${catalog.url}" target="_new"><img class='deploy galleryLink' alt='Browse ${catalog.name}' Title='Browse ${catalog.name}' src='${_gallery.resolveImage(catalog.icon)}' /></a>
                {{/if}}                
                
            </div>
         </div>
	</div>

    </script>
    
    <script id="extDetailTmpl" type="text/html">
	<div class="extension ${catalog.catalogCSS} ext-${extensionID}">    
            <a name="ext-${extensionID}"></a>
            <div class="extDetailTmpl-header">
                <span class="extDetailTmpl-header-left">
                    {{if imageURL}}
    		            <img class='productImageBig' alt='${extensionName}' src="${imageURL}" />
                    {{else}}
    		            <img class='productImageBig' alt='${extensionName}' src='<%=ResolveUrl("~/images/System-Box-Empty-icon.png")%>' />
                    {{/if}}
                </span>
                <span class="extDetailTmpl-header-right">
                    {{if extensionName}}
    		            ${extensionName}
                    {{/if}}
                    {{if MinDnnVersion}}
    		            ${MinDnnVersion}
                    {{/if}}
                    <span class='price'>${_gallery.FormatCurrency(price)}</span>
                </span>
            </div>
            <br/>
                                            
            <img class='productTypeImage dnnIcon' alt='${extensionType}' src='<%=ResolveUrl("~/images/appGallery_${extensionType}.gif")%>' />
            {{if detailURL}}                      
                    <a href="${detailURL}" target="_new"><img alt='Details ${extensionName}' class='productTypeImage dnnIcon' alt='${extensionType}' src='<%=ResolveUrl("~/images/appGallery_details.gif")%>' /></a>
            {{/if}}
            
            <a class="galleryLink inline" href="${detailURL}&PackageOptionID=0&action=Add" target="_new"><img class='dnnIcon shopping-cart galleryLink' alt='Buy ${extensionName}'  Title='Buy ${extensionName}' src='<%=ResolveUrl("~/images/appGallery_cart.gif")%>' /></a>            
            <a class="galleryLink inline"><img class='dnnIcon deploy galleryLink' alt='Deploy ${extensionName}' Title='Deploy ${extensionName}' src='<%=ResolveUrl("~/images/appGallery_deploy.gif")%>' /></a>
            {{if catalog.url}}                
                <a class="galleryLink inline" href="${catalog.url}" target="_new"><img class='deploy galleryLink' alt='Browse ${catalog.name}' Title='Browse ${catalog.name}' src='${_gallery.resolveImage(catalog.icon)}' /></a>
            {{/if}}    
            <div id="extensionDetail-tabs">
	            <ul>
                    {{if Description}}
		                <li><a href="#tabs-details"><%=LocalizeString("DetailsLabel") %></a></li>
                    {{/if}}
                    {{if license}}
    		             <li><a href="#tabs-license"><%=LocalizeString("LicenseLabel") %></a></li>
                    {{/if}}
		           
	            </ul>
                {{if Description}}
	                <div id="tabs-details">
	                    <span class='break-word'>{{html Description}}</span>
                    </div>
                {{/if}}
                {{if license}}
	                <div id="tabs-license">
                    <span class='break-word'>{{html license}}</span>
	                </div>
                {{/if}}
            </div>
        </div>

    </script>
    
    <script type="text/javascript">

        var _gallery; //global scope!
        $(document).ready(function () {
            _gallery = new Gallery(
                {
                    host: '<%=LocalizeString("appgalleryEndpoint") %>'
                    , NameTextASC: '<%=LocalizeString("NameAZ") %>'
                    , NameTextDESC: '<%=LocalizeString("NameZA") %>'
                    , PriceextASC: '<%=LocalizeString("PriceLowHigh") %>'
                    , PriceTextDESC: '<%=LocalizeString("PriceHighLow") %>'
                    , tagLabel: '<%=LocalizeString("TagLabel") %>'
                    , searchLabel: '<%=LocalizeString("SearchLabel") %>'
                    , vendorLabel: '<%=LocalizeString("VendorLabel") %>'
                    , extensionLabel: '<%=LocalizeString("ExtensionsLabel") %>'
                    , noneLabel: '<%=LocalizeString("NoneLabel") %>'
                    , orderLabel: '<%=LocalizeString("OrderLabel") %>'
                    , typeLabel: '<%=LocalizeString("TypeLabel") %>'
                    , errorLabel: '<%=LocalizeString("ErrorLabel") %>'
                    , loadingLabel: '<%=LocalizeString("LoadingLabel") %>'
                    , sortingByLabel: '<%=LocalizeString("SortingByLabel") %>'
                    , imgRoot : "<%=ResolveUrl("~/images/")%>"
                    , DataBaseVersion : "<%=DotNetNuke.Common.Globals.DataBaseVersion%>"

                });
            _gallery.getCatalogs();
            _gallery.getTags();
            _gallery.Search();
        });
    </script>
</div>