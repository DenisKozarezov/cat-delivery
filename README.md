<div align="center">

<h1>Прототип "Cat Delivery"</h1>

[About](#about-cat-delivery)                     •
[Programming Patterns](#programming-patterns)    •
[Screenshots](#screenshots)                      •
[Project Status](#project-status)                •

</div>

## About Cat Delivery

Игровой прототип разработан в рамках джема <a href="https://itch.io/jam/scorejam22">```Game Off 2022```</a> от *GitHub*. Темой конкурса этого года является "***Cli-ché***", интерпретировать тему разрешено как угодно и без ограничений. Время на выполнение: 1 месяц.

Прототип представляет из себя ретро аркаду, вдохновленную играми Sega, в которой игрок управляет антропоморфным персонажем, чьей задачей является поиск пропавших животных и спасение их от похищения. Главный герой обладает пушкой, выстреливающей особыми снарядами, которые при контакте с котами окружают тех защитной сферой. За каждого спасенного кота игрок получает бонусные очки и увеличивает серию без промахов. Соответственно, промах или кража животных данную серию обнуляют.

Цель игры: не позволить похитить при помощи магнита 3-х котов. После проигрыша игрок обновляет рекордное время и свой счет.

Состав команды: геймдизайнер, 2 программиста, художник.

## Programming Patterns

В прототипе используются следующие архитектурные паттерны:
1. ```MVC``` - для управления персонажем и разделения логики от представления (спрайта).
2. ```MVVM (UnityWeld)``` - для связи между игровым состоянием (game state) и UI.
3. ```Dependency Injection (Zenject)``` - для уменьшения связности модулей игры и кода в целом, а также для пробрасывания основных зависимостей.
4. ```IoC``` - абстрагирование при помощи интерфейсов (контрактов) для ухода от конкретных реализаций классов. Используется при создании оружий (*IWeapon*, *LaserGun*, *BulletGun*), а также для пробрасывания глобальных зависимостей (*ICoroutineRunner*, *IPauseProvider*, *ILoadingScreenProvider* и т.д.).
5. ```Factory``` - для инкапсуляции логики создания сущностей в порождающие классы. Примеры фабрик, использующихся в прототипе: *CatView.Factory*, *Laser.Factory*, *Bullet.Factory*, *PlayerFactory*, *ExplosionFactory*.

## Screenshots

## Project Status
