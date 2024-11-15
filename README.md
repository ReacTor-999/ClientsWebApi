# ClientsWebApi

## О проекте
Данный проект - тестовое задание. 

Некоторые особенности:
- В качестве СУБД используется MS SQL Server 2022 в Docker-контейнере на локальной машине
- В проекте реализована аутентификация с использованием JWT-токенов
- API частично задокументирован при помощи SwaggerGen

## Сущности БД

### Client (клиент)

Кратко:
- Требовался по заданию.
- Добавлен только внешний ключ к таблице Users.
- Валидация ИНН на входе при помощи кастомного атрибута на уровне "10 или 12 цифр". Ничего более

### Founder (учредитель)

- Требовался по заданию.
- Валидация ИНН на входе при помощи кастомного атрибута на уровне "12 цифр". Ничего более

### ContactInfo

- Несколько строк для одного клиента. Подразумевается, что можно указать контакты для нескольких отделов, чтобы настроить более удобное взаимодействие с крупными компаниями.
- Включает отдельные столбцы для эл. почты и телефона, предусмотрено дополнительное поле для указания других контактов

### Users + Roles etc

- Встроенное решение. Ничего не изменялось
- UserName указывается как имя почты без доменного имени
- Доступные роли: User(зарегистрированный пользователь), Client (клиент, подвержденный профиль), Manager (Менеджер, может быть создан только администратором), Admin(администратор)

### Contract (Контракт, договор)

- Имеет внешний ключ к клиенту, но не удаляется каскадно. Дополнительно хранит ИНН клиента. Может быть полезно для составления отчетов
- Пока что может быть добавлен только менеджером. В дальнейшем, стоит сделать систему заказов - возможность для клиентов оставлять заказ по существующим предложениям.
- Сохраняет дату заключения договора и (опционально) дату истечения. То есть, может быть бессрочных

### Payment (Платеж)

- Вносится клиентом в уплату по существующему контракту
- Может быть несколько платежей на контракт
- Удаляется каскадно вслед за контрактом

