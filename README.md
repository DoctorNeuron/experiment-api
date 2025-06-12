# Hello There

This is a very simple CRUD backend you can use as a helper for your complete and functional frontend projects.

Just open an issue if you need something to add or something tehe. It already includes migrations adn `web.config` for IIS usage.

The products are based on Hayday products :)

## Turning it up

1. You need to have `postgres`, but feel free to edit the code according to your own DBMS. Make sure you can convert the seeding script accordingly.
2. You need `.NET 8` runtime
3. Migrate your DB using `dotnet ef migrations add InitMigration && dotnet ef database update`
4. Seed your data. Just execute that script. Make sure to add yourself as user. See user management section.
5. Turn it up as usual. Hit `/scalar` for API documentation.

## User Management

To make it very simple, authentication will only require your UserID. Then, it will issue a token according to your needs.

For OAuth user like... say... Microsoft Entra Auth for example, you might need to pass your `oid` from your Microsoft Entra Access Token as your user ID for ease of access.
