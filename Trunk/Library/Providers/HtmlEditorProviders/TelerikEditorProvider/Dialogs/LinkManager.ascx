<%@ Control Language="C#" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI.Editor" TagPrefix="tools" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI.Widgets" TagPrefix="widgets" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI.Dialogs" TagPrefix="dialogs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="DotNetNuke.HtmlEditor.TelerikEditorProvider" Namespace="DotNetNuke.HtmlEditor.TelerikEditorProvider" TagPrefix="provider" %>

<script language="javascript" type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1/jquery.min.js"></script>       
<script type="text/javascript">
    //<!--
    var _PageOnSite_ClientID = "PageOnSite";

    $(function () {
        $("input[name$=TrackLink]").click(function (e) {
            getLinkClickURL();
        });
    });

    $(function () {
        $("input[name$=TrackUser]").click(function (e) {
            var trackUser = document.getElementById("TrackUser");
            if (trackUser.checked == true) {
                var trackLink = document.getElementById("TrackLink");
                trackLink.checked = true;
            }
            getLinkClickURL();
        });
    });

    function _PageOnSite_Change(obj) {
        if (obj) {
            var linkUrlTextBox = document.getElementById("LinkURL");
            var trackLink = document.getElementById("TrackLink");
            if (linkUrlTextBox) {
                if (trackLink.checked == true) {
                    linkUrlTextBox.value = obj.get_value();
                    getLinkClickURL();
                }
                else {
                    linkUrlTextBox.value = obj.get_value();
                }
            }
        }
    }

    function getLinkClickURL() {
        // Data is provided in the DialogParams format
        var linkUrlTextBox = document.getElementById("LinkURL");
        var trackLink = document.getElementById("TrackLink");
        var trackUser = document.getElementById("TrackUser");
        if (linkUrlTextBox.value != 'http://') {
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=UTF-8',
                url: 'LinkClickUrlHandler.ashx',
                data: '{"LinkUrl": "' + linkUrlTextBox.value + '", "Track": "' + trackLink.checked + '", "TrackUser": "' + trackUser.checked + '", "LinkClickUrl": ""}',
                dataType: 'json',
                success: function Success(data) {
                    var linkUrlTextBox = document.getElementById("LinkURL");
                    linkUrlTextBox.value = data.LinkClickUrl;
                },
                error: function (x, status, e) {
                    alert(status);
                    alert(x.responseText);
                }
            });
        }
        else {
            alert("Please enter a valid URL before trying to track it.");
            trackLink.checked = false;
            trackUser.checked = false;
        }
    }           


	Type.registerNamespace("Telerik.Web.UI.Widgets");

	Telerik.Web.UI.Widgets.LinkManager = function(element)
	{
		Telerik.Web.UI.Widgets.LinkManager.initializeBase(this, [element]);
		this._clientParameters = null;
	}

	Telerik.Web.UI.Widgets.LinkManager.prototype = {
	    initialize: function () {
	        Telerik.Web.UI.Widgets.LinkManager.callBaseMethod(this, "initialize");
	        this.setupChildren();
	    },

	    dispose: function () {
	        $clearHandlers(this._linkTargetCombo);
	        this._linkTargetCombo = null;
	        if (this._existingAnchor)
	            $clearHandlers(this._existingAnchor);
	        this._existingAnchor = null;
	        if (this._insertButton)
	            $clearHandlers(this._insertButton);
	        this._insertButton = null;
	        if (this._cancelButton)
	            $clearHandlers(this._cancelButton);
	        this._cancelButton = null;
	        Telerik.Web.UI.Widgets.LinkManager.callBaseMethod(this, "dispose");
	    },

	    clientInit: function (clientParameters) {
	        this._clientParameters = clientParameters;
	        var selectedIndex = this._clientParameters.selectedTabIndex;
	        if (selectedIndex && selectedIndex >= 0) {
	            this._tab.set_selectedIndex(selectedIndex);
	        }
	        //clean
	        this._cleanInputBoxes();
	        this._loadLinkArchor();
	        //load data
	        this._loadLinkProperties();
	    },

	    _cleanInputBoxes: function () {
	        this._linkUrl.value = "";
	        this._linkText.value = "";
	        if (this._linkTargetCombo.options && this._linkTargetCombo.options.length > 0) {
	            this._linkTargetCombo.options[0].selected = true;
	        }
	        this._linkTooltip.value = "";
	        this._anchorName.value = "";
	        this._emailAddress.value = "";
	        this._emailLinkText.value = "";
	        this._emailSubject.value = "";
	        this._linkCssClass.set_value("");
	        this._emailCssClass.set_value("");

	        var pages = $find(_PageOnSite_ClientID);
	        if (pages) pages.clearSelection();
	    },

	    _loadLinkProperties: function () {
	        var currentLink = this._clientParameters.get_value();
	        var currentHref = currentLink.getAttribute("href", 2);
	        var anchors = this._clientParameters.documentAnchors;


	        if (this._clientParameters.showText) {
	            this._linkText.value = currentLink.innerHTML;
	            this._emailLinkText.value = currentLink.innerHTML;

	            if (this._texTextBoxParentNode) this._texTextBoxParentNode.style.display = "";
	            if (this._emailTextBoxParentNode) this._emailTextBoxParentNode.style.display = "";
	        }
	        else {
	            if (this._texTextBoxParentNode) this._texTextBoxParentNode.style.display = "none";
	            if (this._emailTextBoxParentNode) this._emailTextBoxParentNode.style.display = "none";
	        }

	        this._loadCssClasses(currentLink);

	        if (currentHref && currentHref.match(/^(mailto:)([^\?&]*)/ig)) // "email"
	        {
	            this._loadEmailAddressAndSubject();
	            this._tab.set_selectedIndex(2);
	            return;
	        }

	        if (currentLink.name && currentLink.name.trim() != "") // "anchor"
	        {
	            this._anchorName.value = currentLink.name;
	            this._tab.set_selectedIndex(1);
	            return;
	        }

	        var href = "http://"; //"link"

	        if (currentLink.href) {
	            href = currentHref;
	        }
	        var pos = href.toLowerCase().indexOf("linkclick.aspx")
	        if (pos >= 0) {
	            this._trackLink.checked = true;
	        }
	        this._linkUrl.value = href;
	        this._loadLinkTarget();
	        this._linkTooltip.value = currentLink.title;

	        this._tab.set_selectedIndex(0);
	    },

	    _loadLinkTarget: function () {
	        var linkTarget = this._clientParameters.get_value().target;

	        if (!linkTarget) {
	            return;
	        }

	        var optgroups = this._linkTargetCombo.getElementsByTagName("optgroup");

	        for (var i = 0; i < optgroups.length; i++) {
	            if (optgroups[i].nodeName.toLowerCase() == "optgroup") {
	                var options = optgroups[i].getElementsByTagName("option");

	                for (var j = 0; j < options.length; j++) {
	                    if (options[j].nodeName.toLowerCase() == "option" &&
					options[j].value.toLowerCase() == linkTarget.toLowerCase()) {
	                        options[j].selected = true;
	                        return;
	                    }
	                }
	            }
	        }

	        var customOption = document.createElement("option");
	        customOption.value = linkTarget;
	        customOption.text = linkTarget;
	        this._linkTargetCombo.options.add(customOption);
	        customOption.selected = true;
	    },

	    _loadLinkArchor: function () {
	        if (!this._existingAnchor) {
	            return;
	        }
	        var anchors = this._clientParameters.documentAnchors;
	        var linkHref = this._clientParameters.get_value().getAttribute("href", 2) ? this._clientParameters.get_value().getAttribute("href", 2).toLowerCase() : "";
	        //clear existing options
	        this._existingAnchor.innerHTML = "";
	        this._existingAnchor.options.add(new Option(localization["None"], ""));
	        this._existingAnchor.options[0].selected = true;

	        for (var i = 0; i < anchors.length; i++) {
	            var anchorOption = new Option(anchors[i].name, "#" + anchors[i].name);
	            this._existingAnchor.options.add(anchorOption);

	            if ("#" + anchors[i].name.toLowerCase() == linkHref) {
	                anchorOption.selected = true;
	            }
	        }
	    },

	    _loadCssClasses: function (currentLink) {
	        var cssClasses = this._clientParameters.CssClasses;

	        //localization
	        this._linkCssClass.set_showText(true);
	        this._linkCssClass.set_clearclasstext(localization["ClearClass"]);
	        this._linkCssClass.set_text(localization["ApplyClass"]);
	        this._linkCssClass.set_value("");
	        this._emailCssClass.set_showText(true);
	        this._emailCssClass.set_clearclasstext(localization["ClearClass"]);
	        this._emailCssClass.set_text(localization["ApplyClass"]);

	        //Copy the css classes to avoid one collection being modified by the other dropdown
	        this._linkCssClass.set_items(cssClasses.concat([]));
	        this._emailCssClass.set_items(cssClasses);

	        if (currentLink.className != null && currentLink.className != "") {
	            this._linkCssClass.updateValue(currentLink.className);
	            this._emailCssClass.updateValue(currentLink.className);
	        }
	    },

	    _loadEmailAddressAndSubject: function () {
	        var currentHref = this._clientParameters.get_value().getAttribute("href", 2);
	        this._emailAddress.value = RegExp.$2;

	        if (currentHref.match(/(\?|&)subject=([^\b]*)/ig)) {
	            var val = RegExp.$2.replace(/&amp;/gi, "&");
	            val = unescape(val);
	            this._emailSubject.value = val;
	        }
	    },

	    getModifiedLink: function () {
	        var resultLink = this._clientParameters.get_value();
	        var selectedIndex = this._tab.get_selectedIndex();

	        if (selectedIndex == 0)//"link"
	        {
	            resultLink.href = this._linkUrl.value;
	            if (this._linkTargetCombo.value == "_none") {
	                resultLink.removeAttribute("target", 0);
	            }
	            else {
	                resultLink.target = this._linkTargetCombo.value;
	            }

	            if (this._texTextBoxParentNode && this._texTextBoxParentNode.style.display != "none") {
	                resultLink.innerHTML = this._linkText.value;
	            }

	            if (resultLink.innerHTML.trim() == "" || resultLink.innerHTML.trim().length < this._linkText.value.trim().length) {
	                //try to replace <> if the content was marked as invalid html by the browser
	                resultLink.innerHTML = this._linkText.value.replace(/&/gi, "&amp;").replace(/</gi, "&lt;").replace(/>/gi, "&gt;");
	            }

	            if (resultLink.innerHTML.trim() == "") {
	                resultLink.innerHTML = resultLink.href;
	            }

	            if (this._linkTooltip.value.trim() == "") {
	                resultLink.removeAttribute("title", 0);
	            }
	            else {
	                resultLink.title = this._linkTooltip.value;
	            }

	            this._setClass(resultLink, this._linkCssClass);
	        }
	        else if (selectedIndex == 1)//"anchor"
	        {
	            resultLink.removeAttribute("name");
	            resultLink.removeAttribute("NAME");
	            resultLink.name = null;
	            resultLink.name = this._anchorName.value;
	            resultLink["NAME"] = this._anchorName.value;

	            //Make sure the href and some other attributes are removed just in case they are present
	            resultLink.removeAttribute("href");
	            resultLink.removeAttribute("target");
	            resultLink.removeAttribute("title");
	        }
	        else //"email"
	        {
	            resultLink.href = "mailto:" + this._emailAddress.value;

	            if (this._emailSubject.value != "") {
	                resultLink.href += "?subject=" + this._emailSubject.value;
	            }

	            if (this._emailTextBoxParentNode && this._emailTextBoxParentNode.style.display != "none") {
	                resultLink.innerHTML = this._emailLinkText.value;
	            }

	            this._setClass(resultLink, this._emailCssClass);
	        }

	        return resultLink;
	    },

	    _setClass: function (element, cssClassHolder) {
	        if (cssClassHolder.get_value() == "") {
	            element.removeAttribute("className", 0);
	        }
	        else {
	            element.className = cssClassHolder.get_value();
	        }
	    },

	    setupChildren: function () {
	        this._linkUrl = $get("LinkURL");
	        if (this._linkUrl == null) {
	            this._linkUrl = {};
	            this._linkUrl.value = "";
	        }
	        this._linkText = $get("LinkText");

	        this._linkTargetCombo = $get("LinkTargetCombo");
	        this._setLinkTargetLocalization();
	        this._existingAnchor = $get("ExistingAnchor");
	        this._linkTooltip = $get("LinkTooltip");
	        this._trackLink = $get("TrackLink");

	        //NEW: Document manager support
	        this._documentManager = $find("DocumentManagerCaller");

	        this._linkCssClass = $find("LinkCssClass");

	        this._anchorName = $get("AnchorName");

	        this._emailAddress = $get("EmailAddress");
	        this._emailLinkText = $get("EmailLinkText");
	        this._emailSubject = $get("EmailSubject");
	        this._emailCssClass = $find("EmailCssClass");

	        this._insertButton = $get("lmInsertButton");
	        if (this._insertButton)
	            this._insertButton.title = localization["OK"];
	        this._cancelButton = $get("lmCancelButton");
	        if (this._cancelButton)
	            this._cancelButton.title = localization["Cancel"];
	        this._tab = $find("LinkManagerTab");

	        //NEW: In IE RadFormDecorator styles textboxes in a way that the direct parent [of the textbox] changes, so the original implementation stopped working properly (in IE)
	        this._texTextBoxParentNode = $get("texTextBoxParentNode");
	        this._emailTextBoxParentNode = $get("emailTextBoxParentNode");

	        this._initializeChildEvents();
	    },

	    _initializeChildEvents: function () {
	        this._linkCssClass.add_valueSelected(this._cssValueSelected);
	        this._emailCssClass.add_valueSelected(this._cssValueSelected);
	        //NEW: Document manager
	        if (this._documentManager) {
	            this._documentManager.add_valueSelected(Function.createDelegate(this, this._documentManagerClicked));
	            this._documentManager.get_element().title = localization["HyperlinkTab"];
	        }

	        $addHandlers(document, { "keydown": this._keyDownHandler }, this); //NEW add ENTER click handler
	        $addHandlers(this._linkTargetCombo, { "change": this._linkTargetChangeHandler }, this);
	        if (this._existingAnchor)
	            $addHandlers(this._existingAnchor, { "change": this._existingAnchorChangeHandler }, this);
	        if (this._insertButton)
	            $addHandlers(this._insertButton, { "click": this._insertClickHandler }, this);
	        if (this._cancelButton)
	            $addHandlers(this._cancelButton, { "click": this._cancelClickHandler }, this);
	    },

	    //NEW: Document manager
	    _documentManagerClicked: function (oTool, args) {
	        //Editor object is supplied to all dialogs in the dialog parameters
	        var editor = this._clientParameters.editor;
	        var callbackFunction = Function.createDelegate(this, function (sender, args) {
	            //For the time being just set the URL
	            //Returned link - TODO: Use args.get_value() when the dialog returm methods are changed to return proper args object
	            var link = args.get_value ? args.get_value() : args.Result;
	            if (link && link.tagName == "A") {
	                //Set various fileds - classname, target, etc - but only if their value in the returned link is != ""
	                var href = link.getAttribute("href", 2);
	                this._linkUrl.value = href;
	                var trackLink = document.getElementById("TrackLink");
	                if (trackLink.value == true) {
	                    getLinkClickURL();
	                    }
	                if (!this._linkText.value) this._linkText.value = href;
	                var target = link.target;
	                if (target) this._linkTargetCombo.value = target;
	                var title = link.title;
	                if (title) this._linkTooltip.value = title;
	                var className = link.className;
	                if (className) this._linkCssClass.set_value(className);
	            }
	        });
	        var modifiedLink = this.getModifiedLink();
	        var argument = new Telerik.Web.UI.EditorCommandEventArgs("DocumentManager", null, modifiedLink);
	        Telerik.Web.UI.Editor.CommandList._getDialogArguments(argument, "A", editor, "DocumentManager");
	        editor.showDialog("DocumentManager", argument, callbackFunction);
	    },

	    _cssValueSelected: function (oTool, args) {
	        if (!oTool) return;
	        var commandName = oTool.get_name();

	        if ("ApplyClass" == commandName) {
	            var attribValue = oTool.get_selectedItem();
	            oTool.updateValue(attribValue);
	        }
	    },

	    _linkTargetChangeHandler: function (e) {
	        if (this._linkTargetCombo.value == "_custom") {
	            var targetprompttext = "Type Custom Target Here";
	            var targetprompt = prompt(targetprompttext, "CustomWindow");

	            if (targetprompt) {
	                var newoption = document.createElement("option"); // create new <option> node
	                newoption.innerHTML = targetprompt; // set innerHTML to the new <option> none
	                newoption.setAttribute("selected", "selected"); // set the new <option> node selected="selected"
	                newoption.setAttribute("value", targetprompt); // change the value of the new <option> node with the value of the prompt
	                this._linkTargetCombo.getElementsByTagName("optgroup")[1].appendChild(newoption); // append the new <option> node to the <optgroup>
	                return;
	            }

	            this._linkTargetCombo.selectedIndex = 0;
	        }
	    },

	    _setLinkTargetLocalization: function () {
	        var optgroups = this._linkTargetCombo.getElementsByTagName("optgroup");
	        for (var i = 0; i < optgroups.length; i++) {
	            var options = optgroups[i].getElementsByTagName("option");
	            var grpName = optgroups[i].label;
	            if (localization[grpName])
	                optgroups[i].label = localization[grpName];
	            for (var j = 0; j < options.length; j++) {
	                var optName = options[j].text;
	                if (localization[optName])
	                    options[j].text = localization[optName];
	            }
	        }
	    },

	    _existingAnchorChangeHandler: function (e) {
	        if (this._existingAnchor.selectedIndex != 0) {
	            this._linkUrl.value = this._existingAnchor.value;
	        }
	    },

	    _insertClickHandler: function (e) {
	        var modifiedLink = this.getModifiedLink();
	        var args = new Telerik.Web.UI.EditorCommandEventArgs("LinkManager", null, modifiedLink);
	        //backwards compatibility
	        args.realLink = modifiedLink;
	        Telerik.Web.UI.Dialogs.CommonDialogScript.get_windowReference().close(args);
	    },

	    _cancelClickHandler: function (e) {
	        Telerik.Web.UI.Dialogs.CommonDialogScript.get_windowReference().close();
	    },

	    _keyDownHandler: function (e) {
	        if (e.keyCode == 13)
	            this._insertClickHandler(null);
	        else if (e.keyCode == 27)
	            this._cancelClickHandler(e);
	    }
	}

	Telerik.Web.UI.Widgets.LinkManager.registerClass("Telerik.Web.UI.Widgets.LinkManager", Telerik.Web.UI.RadWebControl, Telerik.Web.IParameterConsumer); 
    // -->
</script>



<table cellpadding="0" cellspacing="0" class="reDialog LinkManager NoMarginDialog" style="width: 400px;">
	<tr>
		<td class="reTopcell">
			<telerik:RadTabStrip ShowBaseLine="true" ID="LinkManagerTab" runat="server" SelectedIndex="0" MultiPageID="dialogMultiPage">
				<Tabs>
					<telerik:RadTab Text="HyperlinkTab" Value="HyperlinkTab">
					</telerik:RadTab>
					<telerik:RadTab Text="AnchorTab" Value="AnchorTab">
					</telerik:RadTab>
					<telerik:RadTab Text="EmailTab" Value="EmailTab">
					</telerik:RadTab>
				</Tabs>
			</telerik:RadTabStrip>
		</td>
	</tr>
	<tr>
		<td class="reMiddlecell" style="height: 194px; vertical-align: top;">
			<telerik:RadMultiPage ID="dialogMultiPage" runat="server" SelectedIndex="0">
				<telerik:RadPageView ID="hyperlinkFieldset" runat="server">
					<table border="0" cellpadding="0" cellspacing="0" class="reControlsLayout">
						<asp:PlaceHolder ID="documentCallerRow" runat="server">
							<tr>
						        <td class="reLabelCell"><label for="PageOnSite" class="reDialogLabel"><span>Page</span></label></td>
						        <td class="reControlCell">
						            <provider:PageDropDownList ID="PageOnSite" runat="server" OnClientSelectedIndexChanged="_PageOnSite_Change" />
						        </td>
						    </tr>
							<tr>
								<td class="reLabelCell">
									<label for="LinkURL" class="reDialogLabel">
										<span>
											<script type="text/javascript">document.write(localization["LinkUrl"]);</script>
										</span>
									</label>
								</td>
								<td class="reControlCell">
									<table border="0" cellpadding="" cellspacing="0">
										<tr>
											<td>
												<input type="text" id="LinkURL" style="width: 212px;" />
											</td>
											<td style="padding-left: 4px;">
												<tools:StandardButton runat="server" ToolName="DocumentManager" id="DocumentManagerCaller" />
											</td>
										</tr>
									</table>
								</td>
							</tr>
                            <tr>
                            	<td class="reLabelCell">
								    <label for="TrackLink" class="reDialogLabel">
									    <span>
										    <script type="text/javascript">document.write(localization["TrackLink"]);</script>
									    </span>
								    </label>
							    </td>
                                <td>
                                    <asp:CheckBox ID="TrackLink" runat="server" Text="Track Clicks" />
                                    <asp:CheckBox ID="TrackUser" runat="server" Text="Track User" />
                                </td>
                            </tr>
						</asp:PlaceHolder>
						<tr id="texTextBoxParentNode">
							<td class="reLabelCell">
								<label for="LinkText" class="reDialogLabel">
									<span>
										<script type="text/javascript">document.write(localization["LinkText"]);</script>
									</span>
								</label>
							</td>
							<td class="reControlCell">
								<input type="text" id="LinkText" />
							</td>
						</tr>
						<tr>
							<td class="reLabelCell">
								<label for="LinkTargetCombo" class="reDialogLabel">
									<span>
										<script type="text/javascript">document.write(localization["LinkTarget"]);</script>
									</span>
								</label>
							</td>
							<td class="reControlCell">
								<select id="LinkTargetCombo">
									<optgroup label="PresetTargets">
										<option value="_none">None</option>
										<option value="_self">TargetSelf</option>
										<option value="_blank">TargetBlank</option>
										<option value="_parent">TargetParent</option>
										<option value="_top">TargetTop</option>
										<option value="_search">TargetSearch</option>
										<option value="_media">TargetMedia</option>
									</optgroup>
									<optgroup label="CustomTargets">
										<option value="_custom">AddCustomTarget</option>
									</optgroup>
								</select>
							</td>
						</tr>
						<asp:PlaceHolder ID="existingAnchorRow" runat="server">
							<tr>
								<td class="reLabelCell">
									<label for="ExistingAnchor" class="reDialogLabel">
										<span>
											<script type="text/javascript">document.write(localization["ExistingAnchor"]);</script>
										</span>
									</label>
								</td>
								<td class="reControlCell">
									<select id="ExistingAnchor">
										<option selected="selected">None</option>
									</select>
								</td>
							</tr>
						</asp:PlaceHolder>
						<tr>
							<td class="reLabelCell">
								<label for="LinkTooltip" class="reDialogLabel">
									<span>
										<script type="text/javascript">document.write(localization["LinkTooltip"]);</script>
									</span>
								</label>
							</td>
							<td class="reControlCell">
								<input type="text" id="LinkTooltip" />
							</td>
						</tr>
						<tr>
							<td class="reLabelCell">
								<label for="LinkCssClass" class="reDialogLabel">
									<span>
										<script type="text/javascript">document.write(localization["CssClass"]);</script>
									</span>
								</label>
							</td>
							<td class="reControlCell">
								<tools:ApplyClassDropDown ID="LinkCssClass" runat="server" />
							</td>
						</tr>
					</table>
				</telerik:RadPageView>
				<telerik:RadPageView ID="anchorFieldset" runat="server">
					<table border="0" cellpadding="0" cellspacing="0" class="reControlsLayout">
						<tr>
							<td class="reLabelCell">
								<label for="AnchorName" class="reDialogLabel">
									<span>
										<script type="text/javascript">document.write(localization["LinkName"]);</script>
									</span>
								</label>
							</td>
							<td class="reControlCell">
								<input type="text" id="AnchorName" />
							</td>
						</tr>
					</table>
				</telerik:RadPageView>
				<telerik:RadPageView ID="emailFieldset" runat="server">
					<table border="0" cellpadding="0" cellspacing="0" class="reControlsLayout">
						<tr>
							<td class="reLabelCell">
								<label for="EmailAddress" class="reDialogLabel">
									<span>
										<script type="text/javascript">document.write(localization["LinkAddress"]);</script>
									</span>
								</label>
							</td>
							<td class="reControlCell">
								<input type="text" id="EmailAddress" />
							</td>
						</tr>
						<tr id="emailTextBoxParentNode">
							<td class="reLabelCell">
								<label for="EmailLinkText" class="reDialogLabel">
									<span>
										<script type="text/javascript">document.write(localization["LinkText"]);</script>
									</span>
								</label>
							</td>
							<td class="reControlCell">
								<input type="text" id="EmailLinkText" />
							</td>
						</tr>
						<tr>
							<td class="reLabelCell">
								<label for="EmailSubject" class="reDialogLabel">
									<span>
										<script type="text/javascript">document.write(localization["LinkSubject"]);</script>
									</span>
								</label>
							</td>
							<td class="reControlCell">
								<input type="text" id="EmailSubject" />
							</td>
						</tr>
						<tr>
							<td class="reLabelCell">
								<label for="EmailCssClass" class="reDialogLabel">
									<span>
										<script type="text/javascript">document.write(localization["CssClass"]);</script>
									</span>
								</label>
							</td>
							<td class="reControlCell">
								<tools:ApplyClassDropDown ID="EmailCssClass" runat="server" />
							</td>
						</tr>
					</table>
				</telerik:RadPageView>
			</telerik:RadMultiPage>
		</td>
	</tr>
	<asp:PlaceHolder ID="controlButtonsRow" runat="server">
	<tr>
		<td class="reBottomcell">
			<table border="0" cellpadding="0" cellspacing="0" class="reConfirmCancelButtonsTbl">
				<tr>
					<td>
						<button type="button" id="lmInsertButton">
							<script type="text/javascript">
								setInnerHtml("lmInsertButton", localization["OK"]);
							</script>
						</button>
					</td>
					<td>
						<button type="button" id="lmCancelButton">
							<script type="text/javascript">
								setInnerHtml("lmCancelButton", localization["Cancel"]);
							</script>
						</button>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	</asp:PlaceHolder>
</table>
