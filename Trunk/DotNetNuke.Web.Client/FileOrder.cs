namespace DotNetNuke.Web.Client
{
    /// <summary>
    /// Contains enumerations that define the relative loading order of both JavaScript and CSS files within the framework's registration system.
    /// </summary>
    public class FileOrder
    {
        /// <summary>
        /// Defines load order of key JavaScript files within the framework
        /// </summary>
        public enum Js
        {
            /// <summary>
            /// The default priority (100) indicates that the ordering will be done based on the order in which the registrations are made
            /// </summary>
            DefaultPriority = 100,
            /// <summary>
            /// jQuery (CDN or local file) has the priority of 5
            /// </summary>
// ReSharper disable InconsistentNaming
            jQuery = 5,
// ReSharper restore InconsistentNaming
            /// <summary>
            /// jQuery UI (CDN or local file) has the priority of 10
            /// </summary>
// ReSharper disable InconsistentNaming
            jQueryUI = 10,
// ReSharper restore InconsistentNaming
            /// <summary>
            /// /js/dnn.js has the priority of 15
            /// </summary>
            DnnJs = 15,
            /// <summary>
            /// /js/dnn.xml.js has the priority of 20
            /// </summary>
            DnnXml = 20,
            /// <summary>
            /// /js/dnn.xml.jsparser.js has the priority of 25
            /// </summary>
            DnnXmlJsParser = 25,
            /// <summary>
            /// /js/dnn.xmlhttp.js has the priority of 30
            /// </summary>
            DnnXmlHttp = 30,
            /// <summary>
            /// /js/dnn.xmlhttp.jsxmlhttprequest.js has the pririty of 35
            /// </summary>
            DnnXmlHttpJsXmlHttpRequest = 35,
            /// <summary>
            /// /js/dnn.dom.positioning.js has the priority of 40
            /// </summary>
            DnnDomPositioning = 40,
            /// <summary>
            /// /js/dnn.controls.js has the priority of 45
            /// </summary>
            DnnControls = 45,
            /// <summary>
            /// /js/dnn.controls.labeledit.js has the priority of 50
            /// </summary>
            DnnControlsLabelEdit = 50,
        }

        /// <summary>
        /// Defines load order of key CSS files within the framework
        /// </summary>
        public enum Css
        {
            /// <summary>
            /// The default priority (100) indicates that the ordering will be done based on the order in which the registrations are made
            /// </summary>
            DefaultPriority = 100,
            /// <summary>
            /// The default.css file has a priority of 5
            /// </summary>
            DefaultCss = 5,
            /// <summary>
            /// Module CSS files have a priority of 10
            /// </summary>
            ModuleCss = 10,
            /// <summary>
            /// Skin CSS files have a priority of 15
            /// </summary>
            SkinCss = 15,
            /// <summary>
            /// Specific skin control's CSS files have a priority of 20
            /// </summary>
            SpecificSkinCss = 20,
            /// <summary>
            /// Container CSS files have a priority of 25
            /// </summary>
            ContainerCss = 25,
            /// <summary>
            /// Specific container control's CSS files have a priority of 30
            /// </summary>
            SpecificContainerCss = 30,
            /// <summary>
            /// The portal.css file has a priority of 35
            /// </summary>
            PortalCss = 35,
        }
    }
}