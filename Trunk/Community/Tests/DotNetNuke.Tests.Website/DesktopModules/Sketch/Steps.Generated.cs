using TechTalk.SpecFlow;

namespace DotNetNuke.Tests.Website.DesktopModules.Sketch
{
    [Binding]
    public partial class Steps : AutomationBase
    {

        [When(@"In the Title text box, enter the name of the Sketch (.*)")]
        public void WhenInTheTitleTextBoxEnterTheNameOfTheSketch(string title)
        {
            DesktopModules.Sketch.UI.TitleTextbox(Driver).SendKeys(title);
        }

        [When(@"In the Page Permission grid set the permissions for the Page for this Sketch")]
        public void WhenInThePagePermissionGridSetThePermissionsForThePageForThisSketch()
        {
        }

        [When(@"In the Module Permission grid set the permissions for the Module for this Sketch")]
        public void WhenInTheModulePermissionGridSetThePermissionsForTheModuleForThisSketch()
        {
        }

        [When(@"In the Description field describe the main function of the Sketch and its expected outcome\.")]
        public void WhenInTheDescriptionFieldDescribeTheMainFunctionOfTheSketchAndItsExpectedOutcome_()
        {
            DesktopModules.Sketch.UI.DescriptionTextbox(Driver).SendKeys("This Sketch Rocks even more with a bunch more text added.");
        }

        [When(@"In the Steps field start typing the step and select the Step you wish to use from the autocomplete list")]
        public void WhenInTheStepsFieldStartTypingTheStepAndSelectTheStepYouWishToUseFromTheAutocompleteList()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"In the Examples grid fill out a value for each field")]
        public void WhenInTheExamplesGridFillOutAValueForEachField()
        {
            ScenarioContext.Current.Pending();
        }


        [When(@"Click the Save button")]
        public void WhenClickTheSaveButton()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"A new Sketch node will appear in the Sketches tree")]
        public void ThenANewSketchNodeWillAppearInTheSketchesTree()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"The Sketch editing form will switch to View mode")]
        public void ThenTheSketchEditingFormWillSwitchToViewMode()
        {
            ScenarioContext.Current.Pending();
        }

    }
}
