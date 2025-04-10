using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace lamerstests;

public class Tests
{
    private IWebDriver driver;
    private WebDriverWait wait;
    private const string UserLogin = "xxx";
    private const string UserPassword = "xxx"; 
    const string communityName = "Сообщество через автотест";
    const string communityNameForDelete = "Сообщество для удаления";
    
   [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");

        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
    }

    [Test]
    public void StaffProfilePageOpens()
    {
        TestsHelpers.Authorization(driver, UserLogin, UserPassword, wait);
        
        var profileDropdownButton = By.CssSelector("[data-tid='DropdownButton'] button");
        wait.Until(ExpectedConditions.ElementToBeClickable(profileDropdownButton)).Click();

        var profileMenuItem = By.CssSelector("[data-tid='ScrollContainer__inner'] [data-tid='Profile']");
        wait.Until(ExpectedConditions.ElementToBeClickable(profileMenuItem)).Click();

        var titlePageElement = driver.FindElement(By.CssSelector("[data-tid='PageHeader'] [data-tid='Item']"));
        
        Assert.That(titlePageElement.Text, Does.Contain("Профиль"), "Ошибка: не происходит переход на профиль пользователя");
    }

    [Test]
    public void StaffDeleteCommunity()
        {                   
           TestsHelpers.Authorization(driver, UserLogin, UserPassword, wait);

           TestsHelpers.CreateCommunity(driver, communityNameForDelete, wait);

           var deleteCommunityButton = By.CssSelector("[data-tid='DeleteButton']");
           wait.Until(ExpectedConditions.ElementToBeClickable(deleteCommunityButton)).Click();

           var deleteCommunityModalWindow = By.CssSelector("[data-tid='modal-content']");
           wait.Until(ExpectedConditions.ElementIsVisible(deleteCommunityModalWindow));

           var deleteCommunityButtonYes = By.CssSelector("[data-tid='DeleteButton'] button");
           wait.Until(ExpectedConditions.ElementToBeClickable(deleteCommunityButtonYes)).Click();

           driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities?activeTab=isAdministrator");

           var remainingElements = driver.FindElements(By.XPath($"//a[@data-tid='Link' and text()='{communityNameForDelete}']"));
           Assert.That(remainingElements, Has.Count.EqualTo(0), $"Ошибка: Сообщество '{communityNameForDelete}' не было удалено");
        }

        [Test]
        public void StaffCreateNewCommunity()
        {
            TestsHelpers.Authorization(driver, UserLogin, UserPassword, wait);
            
            TestsHelpers.CreateCommunity(driver, communityName, wait);

            // Assert
            var communityTitle = By.CssSelector("[data-tid='PageHeader'] [data-tid='Title'] a");
            wait.Until(ExpectedConditions.TextToBePresentInElementLocated(communityTitle, communityName));
            Assert.That(driver.FindElement(communityTitle).Text, Is.EqualTo(communityName), $"Ошибка: Сообщество '{communityName}' не было создано");
        }
        [Test]
        public void AuthorizationToStaff()
        {
            TestsHelpers.Authorization(driver, UserLogin, UserPassword, wait);

            var newsTitlePageElement = driver.FindElement(By.CssSelector("[data-tid='PageHeader'] [data-tid='Title']"));
        
            Assert.That(newsTitlePageElement.Text, Does.Contain("Новости"), "Ошибка: не происходит авторизация пользователя");
        }

        [Test]
        public void NavigationMenuElement()
        {
            TestsHelpers.Authorization(driver, UserLogin, UserPassword, wait);

            var commentsLink = By.CssSelector("a[data-tid='Comments']");
            wait.Until(ExpectedConditions.ElementToBeClickable(commentsLink)).Click();

            var commentsTitlePageElement = driver.FindElement(By.CssSelector("[data-tid='PageHeader'] [data-tid='Title']"));
        
            Assert.That(commentsTitlePageElement.Text, Does.Contain("Комментарии"), "Ошибка: не происходит переход в раздел 'Комментарии'");
        }

}

public static class TestsHelpers
{
    public static void Authorization(IWebDriver driver, string userLogin, string userPassword, WebDriverWait wait)
        {        
            driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/login");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            
            var userNameInputSelector = By.CssSelector("#Username");
            driver.FindElement(userNameInputSelector).SendKeys(userLogin);
            
            var userPasswordInputSelector = By.CssSelector("#Password");
            driver.FindElement(userPasswordInputSelector).SendKeys(userPassword);
            
            var loginButtonLocator = By.CssSelector("button[value='login']");
            driver.FindElement(loginButtonLocator).Click();

    }

    public static void CreateCommunity(IWebDriver driver, string communityName, WebDriverWait wait)
        {
            // Переход в сообщества
            var communityLink = By.CssSelector("a[data-tid='Community']");
            wait.Until(ExpectedConditions.ElementToBeClickable(communityLink)).Click();
            wait.Until(ExpectedConditions.UrlContains("communities"));

            // Создание сообщества
            var createButton = By.CssSelector("[data-tid='PageHeader'] button");
            wait.Until(ExpectedConditions.ElementToBeClickable(createButton)).Click();

            var nameInput = By.CssSelector("[data-tid='Name'] textarea");
            wait.Until(ExpectedConditions.ElementIsVisible(nameInput)).SendKeys(communityName);

            var submitButton = By.CssSelector("[data-tid='CreateButton'] button");
            wait.Until(ExpectedConditions.ElementToBeClickable(submitButton)).Click();

    }
    
}