<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Extensions.MoreExtensions" CodeFile="MoreExtensions.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>  
<div id="dnnAppGallery" class="dnnForm dnnAppGallery dnnClear">
    <span id="loading" class="dnnAppGalleryLoading">Loading...</span>
    <div class="header"><h1><%=LocalizeString("AppGallery") %></h1></div>
    <fieldset>
        <legend><%=LocalizeString("AppGallerySearchTitle") %></legend>
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
                <option>Library</option>
                <option selected="selected" value="module"><%=LocalizeString("AppGalleryModule") %></option>
                <option>Provider</option>
                <option value="skin"><%=LocalizeString("AppGallerySkin") %></option>
                <option>Skin Object</option>
                <option>Widget</option>
                <option>Other</option>
            </select>        
        </div>
        <span class="dnnAppGalleryTags"><span class="tags-list" id="tag-list"></span></span>
        <div class="dnnFormItem">
            <label for="searchText">
                <a href="#" onclick="if (__dnn_SectionMaxMin(this,  'searchHelp')) return false;" class="dnnFormHelp" >
                    <span><%=LocalizeString("SearchLabel") %></span>
                </a>
            </label>
            <div id="searchHelp" class="dnnFormHelpContent dnnClear" style="display:none;">
		        <span><%=LocalizeString("SearchLabel.Help")%></span>
	        </div>
            <input type="text" id="searchText" title="<%=LocalizeString("SearchLabel.Help")%>" />        
            <a href="javascript:void(0);" id="search-reset" class="dnnSecondaryAction">Clear Search</a>
            <a href="javascript:void(0);" id="search-go" class="dnnSecondaryAction">Search</a>
        </div>
    </fieldset>
    <div id="extensionDetail">
        <div id="extensionDetailInner"/>
    </div>
    <fieldset id="extensionsSummary">
        <legend><%=LocalizeString("Extensions") %></legend>
        <div class="sort-options fullWidth-Center">
            &nbsp;|&nbsp; <span class="sort-button-off"><a href="javascript:void(0);" id="NameSorter">
                Name: A-Z</a></span> <span class="sort-button-off">&nbsp;|&nbsp;<a href="javascript:void(0);"
                    id="PriceSorter">Price: Low to High</a></span> &nbsp;|&nbsp;
        </div>
        <span id="extensionList" class="extensionList"></span>
    </fieldset>
    <script id="tag-tmpl" type="text/html">
{{if tagName}}
    <span style="font-size:${fontSize}em"><a href="javascript:void(0);" class="tag" value="${tagID}">${tagName} (${count})</a></span>&nbsp;
{{/if}}
    </script>
    <script id="extDetailTmpl" type="text/html">
	<div class="extension ${_gallery.GetRowClass()} ext-${extensionID}">    
            <a name="ext-${extensionID}"></a>
            <div class="extDetailTmpl-header">
                <span class="extDetailTmpl-header-left">
                    {{if imageURL}}
    		            <img class='productImageBig' alt='${extensionName}' src="${imageURL}" />
                    {{else}}
    		            <img class='productImageBig' alt='${extensionName}' src="./images/System-Box-Empty-icon.png" />
                    {{/if}}
                </span>
                <span class="extDetailTmpl-header-right">
                    {{if extensionName}}
    		            ${extensionName}
                    {{/if}}
                    {{if minDnnVersion}}
    		            (${minDnnVersion})
                    {{/if}}
                    <span class='price'>${_gallery.FormatCurrency(price)}</span>
                </span>
            </div>
            <br/>
                                            
            <img class='productTypeImage productIcon' alt='${extensionType}' src="./images/${extensionType}.png" />
            {{if detailURL}}
                    <a href="${detailURL}" target="_new"><img class='productTypeImage productIcon' alt='${extensionType}' src="./images/arrow_up.png" /></a>
            {{/if}}
            <a class="galleryLink inline" href="${detailURL}&PackageOptionID=0&action=Add" target="_new"><img class='productIcon shopping-cart galleryLink' alt='Buy ${extensionName}'  Title='Buy ${extensionName}' src='/images/shopping_cart.png' /></a>
            <a class="galleryLink inline"><img class='productIcon deploy galleryLink' alt='Deploy ${extensionName}' Title='Deploy ${extensionName}' src='/images/deploy.png' /></a>

            <div id="extensionDetail-tabs">
	            <ul>
                    {{if Description}}
		                <li><a href="#tabs-details">Details</a></li>
                    {{/if}}
                    {{if license}}
    		             <li><a href="#tabs-license">License</a></li>
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
    <script id="eTmpl" type="text/html">
	<div>    
	<div class="extension ${_gallery.GetRowClass()} ext-${extensionID}">    
        <a name="ext-${extensionID}"></a>
            {{if imageURL}}
    		    <img class='productImage' alt='${extensionName}' src="${imageURL}" />
            {{else}}
    		    <img class='productImage' alt='${extensionName}' src="http://icons.iconarchive.com/icons/tpdkdesign.net/refresh-cl/256/System-Box-Empty-icon.png" />
            {{/if}}
            {{if extensionName}}
                {{if detailURL}}
		            <a href="javascript:void(0);" class="galleryLink inline" onclick="return _gallery.ShowDetails(${extensionID})">
                {{/if}}
    		        ${extensionName}&nbsp;&nbsp;
                {{if detailURL}}
                    </a>
                {{/if}}            
                {{if minDnnVersion}}
    		        (${minDnnVersion})
                {{/if}}
                {{if ownerName}}
                        by <a href="javascript:void(0)" class="galleryLink  inline" onclick="_gallery.OwnerFilterGallery('${ownerName}')">${ownerName}</a>
                {{/if}}
            {{else}}
                More Information
            {{/if}}
            &nbsp;&nbsp;
            <span class='price'>${_gallery.FormatCurrency(price)}</span>
            <br/>
            
		    <span class='break-word'>{{html Description}}</span><br /><br />
                                
            <img class='productTypeImage productIcon' alt='${extensionType}'  title='${extensionType}'  src="./images/${extensionType}.png" />

            {{if license}}
                <a class="galleryLink  inline" onclick="return Gallery.ShowDetails(${extensionID})">
                    <img class='productTypeImage productIcon' alt='License' title='License Specified' src="./images/license.png" />
                </a>
            {{/if}}
            
            {{if detailURL}}
                    <a href="${detailURL}" target="_new"><img class='productTypeImage productIcon' alt='${extensionType}' src="./images/arrow_up.png" /></a>
            {{/if}}
            <a class="galleryLink inline" href="${detailURL}&PackageOptionID=0&action=Add" target="_new"><img class='productIcon shopping-cart galleryLink' alt='Buy ${extensionName}'  Title='Buy ${extensionName}' src='/images/shopping_cart.png' /></a>
            <a class="galleryLink inline"><img class='productIcon deploy galleryLink' alt='Deploy ${extensionName}' Title='Deploy ${extensionName}' src='/images/deploy.png' /></a>
    <hr width='100%' size='1' />
        </div>
	</div>

    </script>
    <script type="text/javascript">

        var _gallery; //global scope!
        $(document).ready(function () {
             _gallery = new Gallery({
                host: '<%=LocalizeString("appgalleryEndpoint") %>'
             });
            _gallery.Search();
        });
    </script>
</div>