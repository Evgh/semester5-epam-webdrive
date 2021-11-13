﻿using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using _5ERAT8.Pages;

namespace _5ERAT8
{
    [TestFixture]
    class Test
    {
        WebDriver driver;

        [OneTimeSetUp]
        public void Startup()
        {
            driver = new EdgeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
        }

        [Test]
        public void CreateNewPaste()
        {
            var createNewPastePage = new CreateNewPastePage(driver);

            createNewPastePage.OpenPage();
            createNewPastePage.CodeInputForm.SendKeys("Hello from WebDriver");
            createNewPastePage.SetPasteExpiration();
            createNewPastePage.SetPasteName("helloweb");

            var pasteCreatedPage = createNewPastePage.CreatePaste();
        }

        [Test]
        public void CreateNewBashPaste()
        {
            var createNewPastePage = new CreateNewPastePage(driver);
            createNewPastePage.OpenPage();

            var code = "git config --global user.name  \"New Sheriff in Town\"\r\ngit reset $(git commit-tree HEAD^{tree} -m \"Legacy code\")\r\ngit push origin master --force";
            var title = "how to gain dominance among developers";

            createNewPastePage.CodeInputForm.SendKeys(code);
            createNewPastePage.SetPasteExpiration();
            createNewPastePage.SetSyntaxHighlighting();
            createNewPastePage.SetPasteName(title);

            PasteCreatedPage pasteCreatedPage = createNewPastePage.CreatePaste();

            Assert.AreEqual(pasteCreatedPage.RawPasteDataText.Text, code);
            Assert.AreEqual(pasteCreatedPage.HighlightLanguage.Text, "Bash");
            Assert.AreEqual(pasteCreatedPage.Title.Text, title);
            Assert.AreEqual(pasteCreatedPage.ExpirationTime.Text, "10 MIN");
        }

        [Test]
        public void EmailedEstimatedPricesIsTheSameAsOnForm()
        {
            var pageObject = new GoogleCloudPage(driver);
            pageObject.OpenPage().
            WriteToSearchBox("Pricing calculator").
            SelectSearchBoxOption("Google Cloud Platform Pricing Calculator").
            SwitchToCalculatorIFrame().
            AddNumberOfInstances(4).
            SelectMachineTypeStandard2().
            AddNodesCount(1).
            CheckAddGPUs().
            SelectNvidiaTeslaP4().
            Select4NumberOfGPUs().
            Select24x375GbSSDs().
            SelectFrankfurtLocation().
            SelectCommitedUsage1Year().
            AddToEstimates();

            var expectedPrice = pageObject.GetEstimatePrice();
            pageObject.OpenNewTab();


            driver.SwitchTo().Window(driver.WindowHandles[1]);

            var yopMailPageObject = new YopMailPage(driver);
            var emailName = Guid.NewGuid().ToString().Remove(24);
            yopMailPageObject.OpenPage().EnterEmailName(emailName).CreateEmail();
            driver.SwitchTo().Window(driver.WindowHandles[0]);
            pageObject.SwitchToCalculatorIFrame().EmailEstimate(emailName + "@yopmail.com");
            Thread.Sleep(4000);
            driver.SwitchTo().Window(driver.WindowHandles[1]);

            var actualPrice = yopMailPageObject.RefreshMail().GetEstimatedPriceFromEmail();

            Assert.AreEqual(expectedPrice, actualPrice);
        }
    }
}