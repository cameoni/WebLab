1. Название проекта
Управление сущностью Teacher (вариант 8)

2. Описание предметной области
Данная программа предоставляет возможности по работе с сущностью Teacher, имеющей следующие поля:
Id - уникальный идентификатор
FullName - полное имя, строка от 1 до 120 символов, обязательное поле
Department - название департамента, строка до 200 символов
Email - валидный Email-адрес, строка до 200 символов
ExperienceYears - стаж, число от 0 до 100
Active - активность, true/false

3. Стек технологий
С#, PostgreSQL, NET 8.0, ASP.NET Core, Entity Framework Core 9.0.4, JwtBearer 8.0.25, NewtonsoftJson 8.0.25.

4. Как запустить проект
Открыть файл WebLab.sln в Visual Studio и нажать F5

5. Описание маршрутов API

Авторизация:
POST /auth?Login=User&Password=User - попытка авторизации, возвращает 200 Ok и access_token (401 Unauthorized при неверно введённых данных)
POST /auth/login?Login=User&Password=User - попытка авторизации, возвращает 200 Ok и access_token (401 Unauthorized при неверно введённых данных)
POST /auth/logout - возвращает 200 Ok

Работа с сущностью у авторизованного пользователя (передача JWT-токена в Headers) без прав редактирования:
GET /entities - возвращает 200 Ok и teacher_list
GET /entities/{id} - где id = id преподавателя.
Если преподаватель не существует, возвращает 404 Not Found
Если преподаватель существует, возвращает 200 Ok и teacher_data
POST /entities/{id} - возвращает 405 Method Not Allowed
PUT, PATCH, DELETE /entites/{id} - возвращает 403 Not Allowed

Работа с сущностью у авторизованного пользователя (передача JWT-токена в Headers) с правами редактирования:
GET /entities - возвращает 200 Ok и teacher_list
PUT, PATCH, DELETE /entities - возвращает 405 Method Not Allowed
GET /entities/{id} - где id = id преподавателя.
Если преподаватель не существует, возвращает 404 Not Found
Если преподаватель существует, возвращает 200 Ok и teacher_data

POST /entities?FullName={FullName}&Department={Department}&Email={Email}&ExperienceYears={Years}&Active={Active}
Создаёт нового учителя, возвращает 201 Created, при ошибке валидации 422 Unprocessable Entity
FullName - полное имя, строка от 1 до 120 символов, обязательное поле
Department - название департамента, строка до 200 символов
Email - валидный Email-адрес, строка до 200 символов
ExperienceYears - стаж, число от 0 до 100
Active - активность, true/false

PUT /entities/{id}?FullName={FullName}&Department={Department}&Email={Email}&ExperienceYears={Years}&Active={Active}
Обновляет сущность и возвращает 200 Ok (404 Not Found если id не найден, 422 при ошибке валидации)

PATCH /entities/{id},
 в Headers Content-Type:application/json-patch+json
 в Body [{
    "op": "replace", 
    "path": "/{название заменяемого поля}",
    "value": "{значение, на которое производится замена}"
}]
Обновляет значение в одном из полей и возвращает 200 Ok (404 Not Found если id не найден, 400 Bad Request при некорректном запросе)

DELETE /entites/{id} - возвращает 204 No Content (404 Not Found если id не найден)

6. Примеры запросов и ответов
Запрос: POST https://localhost:7060/auth/login?Login=Admin&Password=Admin
Ответ: 200 Ok
{
    "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUcnVlIiwiZXhwIjoxNzc0NDQzNDA5LCJpc3MiOiJUZWFjaGVyQXBwIiwiYXVkIjoiVGVhY2hlckFwcENsaWVudCJ9.RQ6CpQQ-n11EXKJTA_bkWgGb4-kU2i2ZKikh9FUn_EA"
}
Запрос: GET https://localhost:7060/entities
Ответ: 200 Ok
{
    "teacher_list": [
        {
            "Id": 1,
            "FullName": "Иван Иванов Иванович",
            "Department": "Институт Цифровых Систем",
            "Email": "ivanovii@mail.ru",
            "ExperienceYears": 10,
            "Active": true
        },
        {
            "Id": 2,
            "FullName": "Федоров Федор Федорович",
            "Department": "Институт Архитектуры и Дизайна",
            "Email": "fedorovff@mail.ru",
            "ExperienceYears": 7,
            "Active": true
        },
        {
            "Id": 3,
            "FullName": "Александрова Александра Александровна",
            "Department": "Институт Химии и Химических Технологий",
            "Email": "alexandrovaaa@mail.ru",
            "ExperienceYears": 14,
            "Active": false
        }
    ]
}
Запрос: GET https://localhost:7060/entities/3
Ответ: 200 Ok
{
    "teacher_data": {
        "Id": 3,
        "FullName": "Александрова Александра Александровна",
        "Department": "Институт Химии и Химических Технологий",
        "Email": "alexandrovaaa@mail.ru",
        "ExperienceYears": 14,
        "Active": false
    }
}
Запрос: POST https://localhost:7060/entities?FullName=Тополев Евгений Петрович&Department=Департамент&Email=a@mail.ru&ExperienceYears=3&Active=true
Ответ: 201 Created
Запрос: PUT https://localhost:7060/entities/4?FullName=Тополев Евг Петрович&Department=Департамент&Email=a@mail.ru&ExperienceYears=3&Active=true
Ответ: 200 Ok
Запрос: DELETE https://localhost:7060/entities/4
Ответ: 204 No Content

7. Примеры ошибок
GET, PUT, PATCH, DELETE /auth - возвращает 405 Method Not Allowed
GET, PUT, PATCH, DELETE /auth/login - возвращает 405 Method Not Allowed
GET, PUT, PATCH, DELETE /auth/logout - возвращает 405 Method Not Allowed
PUT, PATCH, DELETE /entities - возвращает 405 Method Not Allowed
POST /entities/{id} - возвращает 405 Method Not Allowed
При отсутствии подключения к базе данных 500 Internal Server Error
При попытке GET /entities без авторизации или авторизации с устаревшим токеном - 401 Unauthorized
При попытке POST /entities без авторизации от лица пользоавтеля с разрешенным редактированием - 403 Forbidden
При введении некорректного запроса - 400 Bad Request
POST /entities?FullName={FullName}&Department={Department}&Email={Email}&ExperienceYears={Years}&Active={Active} - при ошибке валидации данных (к примеру, имя более 120 символов) 422 Unprocessable Entity
При попытке GET /entities/{id} и введённого id не существует в базе данных - 404 Not Found

8. Описание структуры проекта
Ссылка на Git: https://github.com/cameoni/WebLab/
