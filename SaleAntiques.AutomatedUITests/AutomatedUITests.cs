using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Drawing;
using System.Threading;
using Xunit;

namespace SaleAntiques.AutomatedUITests
{
    public class AutomatedUITests : IDisposable
    {
        private readonly IWebDriver _driver;
        public AutomatedUITests()
        {
            _driver = new ChromeDriver();
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        [Fact]
        public void ViewRegister_NotUnique()
        {
            _driver.Navigate()
                .GoToUrl("https://localhost:44399/Account/Register");

            _driver.Manage().Window.Size = new Size(1920, 1080);
            _driver.Manage().Window.FullScreen();

            _driver.FindElement(By.Id("Email")).SendKeys("lyaysan@mail.ru");
            _driver.FindElement(By.Id("UserName")).SendKeys("Ляйсан");
            _driver.FindElement(By.Id("TelegramId")).SendKeys("lyaysanismagilova");
            _driver.FindElement(By.Id("Password")).SendKeys("987-654lL");
            _driver.FindElement(By.Id("PasswordConfirmation")).SendKeys("987-654lL");

            Thread.Sleep(5000);
            var element = _driver.FindElement(By.Id("create"));
            new Actions(_driver).MoveToElement(element).Perform();
            element.Click();

            Thread.Sleep(5000);
            Assert.Contains("Уже есть пользователь с таким Telegram-идентификатором", _driver.PageSource);
        }

        [Fact]
        public void ViewRegister()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/Account/Register");

            _driver.Manage().Window.FullScreen();

            _driver.FindElement(By.Id("Email")).SendKeys("lyaysan17@mail.ru");
            _driver.FindElement(By.Id("UserName")).SendKeys("Линар");
            _driver.FindElement(By.Id("TelegramId")).SendKeys("");
            _driver.FindElement(By.Id("Password")).SendKeys("987-654lL");
            _driver.FindElement(By.Id("PasswordConfirmation")).SendKeys("987-654lL");

            Thread.Sleep(10000);
            _driver.FindElement(By.Id("create")).Click();

            Thread.Sleep(10000);
            Assert.Contains("Регистрация прошла успешно", _driver.PageSource);
        }

        [Fact]
        public void ViewAntiques()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399");

            _driver.Manage().Window.FullScreen();

            var element = _driver.FindElement(By.Id("name"));
            new Actions(_driver).MoveToElement(element).Perform();
            element.Click();

            _driver.FindElement(By.Id("Login")).SendKeys("lyaysan12@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("987-654lLP");

            Thread.Sleep(10000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(10000);
            Assert.Equal("Выбранный антиквариат", _driver.Title);
        }

        [Fact]
        public void ViewBuy()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/User/OpenAntiques/7a8c50ee-e905-44b7-bf01-c3f7eea10dc6");

            _driver.Manage().Window.FullScreen();

            _driver.FindElement(By.Id("Login")).SendKeys("lyaysan12@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("987-654lLP");
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(10000);
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("$('#buy').click()");

            Thread.Sleep(10000);
            Assert.Contains("Поздравляем с покупкой!!!", _driver.PageSource);
        }

        [Fact]
        public void ViewAdd()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/User/OpenAntiques/8b1d128a-2701-41cb-8b1b-96cb6173fef7");

            _driver.Manage().Window.FullScreen();

            _driver.FindElement(By.Id("Login")).SendKeys("lyaysan12@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("987-654lLP");
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(10000);
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("$('#Add').click()");

            Thread.Sleep(10000);
            Assert.Contains("Удалить из избранного", _driver.PageSource);
        }

        [Fact]
        public void ViewDelete()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/User/OpenAntiques/8b1d128a-2701-41cb-8b1b-96cb6173fef7");

            _driver.Manage().Window.FullScreen();

            _driver.FindElement(By.Id("Login")).SendKeys("lyaysan12@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("987-654lLP");
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(10000);
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("$('#Delete').click()");

            Thread.Sleep(10000);
            Assert.Contains("Добавить в избранное", _driver.PageSource);
        }


        [Fact]
        public void ViewListBuy()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/Account/Personal");

            _driver.Manage().Window.FullScreen();

            _driver.FindElement(By.Id("Login")).SendKeys("lyaysan2001@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("987-654lL");
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(10000);
            var element = _driver.FindElement(By.Id("MyOrders"));
            new Actions(_driver).MoveToElement(element).Perform();
            element.Click();

            Thread.Sleep(10000);
            Assert.Equal("Список покупок", _driver.Title);
        }


        [Fact]
        public void ViewListMySaved()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399");

            var element = _driver.FindElement(By.Id("Log"));
            new Actions(_driver).MoveToElement(element).Perform();
            element.Click();

            _driver.Manage().Window.FullScreen();

            _driver.FindElement(By.Id("Login")).SendKeys("lyaysan2001@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("987-654lL");
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(10000);
            var element_list = _driver.FindElement(By.Id("SavedList"));
            new Actions(_driver).MoveToElement(element_list).Perform();
            element_list.Click();

            Thread.Sleep(10000);
            Assert.Equal("Избранное", _driver.Title);
        }


        [Fact]
        public void ViewChangePassword()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/Account/Personal");

            _driver.Manage().Window.FullScreen();

            _driver.FindElement(By.Id("Login")).SendKeys("lyaysan12@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("987-654lLP");
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(10000);
            var element = _driver.FindElement(By.Id("ChangePassword"));
            new Actions(_driver).MoveToElement(element).Perform();
            element.Click();

            Thread.Sleep(10000);
            _driver.FindElement(By.Id("OldPassword")).SendKeys("987-654lLP");
            _driver.FindElement(By.Id("NewPassword")).SendKeys("987-654lL");
            _driver.FindElement(By.Id("NewPasswordConfirmation")).SendKeys("987-654lL");
            Thread.Sleep(10000);

            var element_save = _driver.FindElement(By.Id("Save"));
            new Actions(_driver).MoveToElement(element_save).Perform();
            element_save.Click();

            Assert.Contains("Изменения сохранены", _driver.PageSource);
        }


        [Fact]
        public void ViewModerAntiques()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/Moderator");

            _driver.Manage().Window.FullScreen();

            _driver.FindElement(By.Id("Login")).SendKeys("andrey@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("987-654lL");
            _driver.FindElement(By.Id("Go")).Click();

            var element = _driver.FindElement(By.Id("namemod"));
            new Actions(_driver).MoveToElement(element).Perform();
            element.Click();

            Thread.Sleep(10000);
            Assert.Equal("Выбранный антиквариат", _driver.Title);
        }


        [Fact]
        public void ViewAcceptedAntiques()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/Moderator/CheckAntiques/54a514d0-1da1-4e7e-8f37-ff32b5c2796e");

            _driver.Manage().Window.FullScreen();

            _driver.FindElement(By.Id("Login")).SendKeys("andrey@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("987-654lL");
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(10000);
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("$('#accepted').click()");

            Assert.Equal("Перенаправление...", _driver.Title);
        }

        [Fact]
        public void ViewRejectedAntiques()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/Moderator/CheckAntiques/b3f17d18-eb9c-4c78-bee8-ec7a67c80a96");

            _driver.Manage().Window.FullScreen();

            _driver.FindElement(By.Id("Login")).SendKeys("andrey@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("987-654lL");
            _driver.FindElement(By.Id("Go")).Click();

            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("$('#collapseExample').addClass('show')");

            IJavaScriptExecutor js_note = (IJavaScriptExecutor)_driver;
            js_note.ExecuteScript("$('#note').text('Данный антиквариат не соответствует требуемому описанию')");

            Thread.Sleep(3000);

            IJavaScriptExecutor js_button = (IJavaScriptExecutor)_driver;
            js_button.ExecuteScript("$('button[type=\"submit\"]').click()");

            Assert.Equal("Перенаправление...", _driver.Title);
        }

        [Fact]
        public void ViewAutorization()
        {
            _driver.Navigate()
                .GoToUrl("https://localhost:44399/Account/Login");

            _driver.Manage().Window.FullScreen();
            _driver.Manage().Window.Size = new Size(1920, 1080);

            _driver.FindElement(By.Id("Login")).SendKeys("ashpantsev@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("Ans080500");

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(5000);
            Assert.Equal("Список антиквариата", _driver.Title);
        }

        [Fact]
        public void ViewAutorizationWithoutRegister()
        {
            _driver.Navigate()
                .GoToUrl("https://localhost:44399/Account/Login");

            _driver.Manage().Window.FullScreen();
            _driver.Manage().Window.Size = new Size(1920, 1080);

            _driver.FindElement(By.Id("Login")).SendKeys("ashpantsev@gmail.com");
            _driver.FindElement(By.Id("Password")).SendKeys("Ans08052000");

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(5000);
            Assert.Contains("Неверный логин или пароль", _driver.PageSource);
        }

        [Fact]
        public void ViewPersonal()
        {
            _driver.Navigate()
                .GoToUrl("https://localhost:44399/Account/Login");

            _driver.Manage().Window.FullScreen();
            _driver.Manage().Window.Size = new Size(1920, 1080);

            _driver.FindElement(By.Id("Login")).SendKeys("ashpantsev@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("Ans080500");

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Personal")).Click();

            Thread.Sleep(5000);
            Assert.Equal("Личный кабинет", _driver.Title);
        }

        [Fact]
        public void ViewQuit()
        {
            _driver.Navigate()
                .GoToUrl("https://localhost:44399/Account/Personal");

            _driver.Manage().Window.FullScreen();
            _driver.Manage().Window.Size = new Size(1920, 1080);

            _driver.FindElement(By.Id("Login")).SendKeys("ashpantsev@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("Ans080500");

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Quit")).Click();

            Thread.Sleep(5000);
            Assert.Contains("Вход", _driver.PageSource);
        }


        [Fact]
        public void ViewRegisterModer()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/Admin/CreateUser");

            _driver.Manage().Window.FullScreen();
            _driver.Manage().Window.Size = new Size(1920, 1080);


            _driver.FindElement(By.Id("Login")).SendKeys("admin1");
            _driver.FindElement(By.Id("Password")).SendKeys("Qwerty123");
            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Email")).SendKeys("moder1@mail.ru");
            _driver.FindElement(By.Id("UserName")).SendKeys("moder1");
            _driver.FindElement(By.Id("Password")).SendKeys("Qwerty123");
            _driver.FindElement(By.Id("PasswordConfirmation")).SendKeys("Qwerty123");

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("create")).Click();

            Thread.Sleep(5000);
            Assert.Equal("Список модераторов", _driver.Title);
        }

        [Fact]
        public void ViewModerList()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/Admin/UsersList");

            _driver.Manage().Window.FullScreen();
            _driver.Manage().Window.Size = new Size(1920, 1080);

            _driver.FindElement(By.Id("Login")).SendKeys("admin1");
            _driver.FindElement(By.Id("Password")).SendKeys("Qwerty123");
            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(5000);
            Assert.Equal("Список модераторов", _driver.Title);
        }

        [Fact]
        public void ViewCreateAntiques()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/Account/Personal");

            _driver.Manage().Window.FullScreen();
            _driver.Manage().Window.Size = new Size(1920, 1080);

            _driver.FindElement(By.Id("Login")).SendKeys("ashpantsev@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("Ans080500");

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("addAntiques")).Click();


            _driver.FindElement(By.Id("name")).SendKeys("Ружье");
            _driver.FindElement(By.Id("description")).SendKeys("Описание ружья");
            _driver.FindElement(By.Id("price")).SendKeys("20000");
            _driver.FindElement(By.Id("photo")).SendKeys("C:\\Users\\User\\Desktop\\картинки\\8ec9332a6bd0bc22e989cda982063bd5.jpeg");

            Thread.Sleep(10000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(10000);
            Assert.Contains("Антиквариат успешно создан", _driver.PageSource);
        }

        [Fact]
        public void ViewAccessAntiquesList()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399");

            _driver.Manage().Window.FullScreen();
            _driver.Manage().Window.Size = new Size(1920, 1080);

            Thread.Sleep(5000);
            Assert.Contains("Дата", _driver.PageSource);
            Assert.Contains("Цена", _driver.PageSource);
            Assert.Contains("Продавец", _driver.PageSource);
        }

        [Fact]
        public void ViewModerAntiquesList()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/Moderator");

            _driver.Manage().Window.FullScreen();
            _driver.Manage().Window.Size = new Size(1920, 1080);

            _driver.FindElement(By.Id("Login")).SendKeys("moder");
            _driver.FindElement(By.Id("Password")).SendKeys("Qwerty123");
            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(5000);
            Assert.Contains("Продавец", _driver.PageSource);
        }

        [Fact]
        public void ViewEditAntiques()
        {
            _driver.Navigate()
            .GoToUrl("https://localhost:44399/Account/Personal");

            _driver.Manage().Window.FullScreen();
            _driver.Manage().Window.Size = new Size(1920, 1080);

            _driver.FindElement(By.Id("Login")).SendKeys("ashpantsev@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("Ans080500");

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Edit")).Click();

            _driver.FindElement(By.Id("name")).Clear();
            _driver.FindElement(By.Id("description")).Clear();
            _driver.FindElement(By.Id("price")).Clear();
            _driver.FindElement(By.Id("photo")).Clear();

            _driver.FindElement(By.Id("name")).SendKeys("Ружье");
            _driver.FindElement(By.Id("description")).SendKeys("Описание ружья");
            _driver.FindElement(By.Id("price")).SendKeys("20000");
            _driver.FindElement(By.Id("photo")).SendKeys("C:\\Users\\User\\Desktop\\картинки\\8ec9332a6bd0bc22e989cda982063bd5.jpeg");

            Thread.Sleep(10000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(10000);
            Assert.Contains("Данные обновлены", _driver.PageSource);
        }

        [Fact]
        public void ViewUpdatePersonalData()
        {
            _driver.Navigate()
                .GoToUrl("https://localhost:44399/Account/Personal");

            _driver.Manage().Window.FullScreen();
            _driver.Manage().Window.Size = new Size(1920, 1080);

            _driver.FindElement(By.Id("Login")).SendKeys("ashpantsev@mail.ru");
            _driver.FindElement(By.Id("Password")).SendKeys("Ans080500");

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("Go")).Click();

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("update")).Click();

            _driver.FindElement(By.Id("NewEmail")).SendKeys("ashpantsev@gmail.com");
            _driver.FindElement(By.Id("NewTelegramUserName")).SendKeys("bigdruse300");

            Thread.Sleep(5000);
            _driver.FindElement(By.Id("save")).Click();

            Thread.Sleep(5000);
            Assert.Contains("ashpantsev@gmail.com", _driver.PageSource);
            Assert.Contains("bigdruse300", _driver.PageSource);
        }
    }
}