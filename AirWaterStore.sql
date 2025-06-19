USE master
GO

-- Create database
CREATE DATABASE AirWaterStore;
GO

USE AirWaterStore;
GO

-- Users table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(MAX) NOT NULL,
    Role INT NOT NULL, -- '1 - Customer' or '2 - Staff'
	IsBan BIT Default 0
);
GO

-- Games table
CREATE TABLE Games (
    GameId INT IDENTITY(1,1) PRIMARY KEY,
    ThumbnailUrl NVARCHAR(255),
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Genre NVARCHAR(200),
    Developer NVARCHAR(100),
    Publisher NVARCHAR(100),
    ReleaseDate DATE,
    Price DECIMAL(10, 2) NOT NULL,
	Quantity INT NOT NULL
);
GO

-- Orders table
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalPrice DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(20) NOT NULL, -- 'Pending', 'Completed'
    CONSTRAINT FK_Orders_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

-- OrderDetails table
CREATE TABLE OrderDetails (
    OrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    GameId INT NOT NULL,
	Quantity INT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    CONSTRAINT FK_OrderDetails_Order FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    CONSTRAINT FK_OrderDetails_Game FOREIGN KEY (GameId) REFERENCES Games(GameId)
);
GO

-- Reviews table
CREATE TABLE Reviews (
    ReviewId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    GameId INT NOT NULL,
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(MAX),
    ReviewDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Reviews_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Reviews_Game FOREIGN KEY (GameId) REFERENCES Games(GameId)
);
GO

CREATE TABLE ChatRooms (
    ChatRoomId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    StaffId INT NULL,
    CONSTRAINT FK_Conversations_Customer FOREIGN KEY (CustomerId) REFERENCES Users(UserId),
    CONSTRAINT FK_Conversations_Staff FOREIGN KEY (StaffId) REFERENCES Users(UserId)
);
GO

CREATE TABLE Messages (
    MessageId INT IDENTITY(1,1) PRIMARY KEY,
    ChatRoomId INT NOT NULL,
    UserId INT NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    SentAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Messages_Conversation FOREIGN KEY (ChatRoomId) REFERENCES ChatRooms(ChatRoomId),
    CONSTRAINT FK_Messages_Sender FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

-- Insert Users
INSERT INTO Users (Username, Email, Password, Role) VALUES 
('Alice Wonder', 'alice@gmail.com', '123456', 1),
('Bob Giser', 'bob@gmail.com', '123456', 1),
('CuteDimple665', 'dimple@gmail.com', '123456', 1),
('Dani256', 'dani@gmail.com', '123456', 1),
('Jeffy Drone', 'jeffy@gmail.com', '123456', 1),
('Just Monika', 'monika@gmail.com', '123456', 1),
('Staff Jr.', 'staff@gmail.com', '123456', 2),
('Robert Downey', 'robert@gmail.com', '123456', 2);

-- Insert Games
INSERT INTO Games (ThumbnailUrl, Title, Description, Genre, Developer, Publisher, ReleaseDate, Price, Quantity) VALUES 
('https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1245620/header.jpg?t=1748630546', 'Elden Ring', 'THE CRITICALLY ACCLAIMED FANTASY ACTION RPG. Rise, Tarnished, and be guided by grace to brandish the power of the Elden Ring and become an Elden Lord in the Lands Between.',
'Souls-like, Open World, Dark Fantasy, RPG', 'FromSoftware, Inc', 'FromSoftware, Inc., Bandai Namco Entertainment', '2022-02-25', 69.99, 100),
('https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1145360/header.jpg?t=1715722799', 'Hades', 'Defy the god of the dead as you hack and slash out of the Underworld in this rogue-like dungeon crawler from the creators of Bastion, Transistor, and Pyre.', 
'Rogue-like, Hack and Slash, Dungeon Crawler.', 'Supergiant Games', 'Supergiant Games', '2020-09-17', 24.99, 50),
('https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1062090/4bc77318229981d441aa33fad4959c684116f599/header.jpg?t=1748943231', 'Timberborn', 'Humans are long gone. In a world struck by droughts and toxic waste, will your lumberpunk beavers do any better? A city-building game featuring ingenious animals, vertical architecture, water physics, and terraforming. Contains high amounts of wood.',
'City Builder, Voxel, Colony Sim, Nature, Sandbox', 'Mechanistry', 'Mechanistry', '2021-09-15', 20.99, 60),
('https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/2420510/header.jpg?t=1740642230', 'HoloCure - Save the Fans!', 'Play as your favorite Vtubers from Hololive! Fight, explore, and clear your way through armies of fans and save them from their mind-control in this unofficial free fan-game.',
'Pixel Graphics, Anime, Bullet Hell, Action Roguelike, Cute', 'KayAnimate', 'KayAnimate', '2023-08-17', 0.99, 120),
('https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/413150/header.jpg?t=1711128146', 'Stardew Valley', 'You haveve inherited your grandfathers old farm plot in Stardew Valley. Armed with hand-me-down tools and a few coins, you set out to begin your new life. Can you learn to live off the land and turn these overgrown fields into a thriving home?',
'Farming Sim, Pixel Graphics, Multiplayer, Life Sim, RPG', 'ConcernedApe', 'ConcernedApe', '2016-02-27', 8.99, 30),
('https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/2358720/header.jpg?t=1749182199', 'Black Myth: Wukong', 'Black Myth: Wukong is an action RPG rooted in Chinese mythology. You shall set out as the Destined One to venture into the challenges and marvels ahead, to uncover the obscured truth beneath the veil of a glorious legend from the past.',
'Mythology, Souls-like, Singleplayer, RPG', 'Game Science', 'Game Science', '2024-08-20', 79.99, 72);

-- Insert Orders
INSERT INTO Orders (UserId, TotalPrice, Status) VALUES 
(1, 69.99, 'Completed'),
(1, 24.99, 'Pending');

-- Insert OrderDetails
INSERT INTO OrderDetails (OrderId, GameId, Quantity, Price) VALUES 
(1, 1, 1, 69.99),
(2, 2, 1, 24.99);

-- Insert Reviews
INSERT INTO Reviews (UserId, GameId, Rating, Comment) VALUES 
(1, 1, 5, 'Absolutely incredible game!'),
(2, 1, 4, 'I was never ever interested in souls like games until I opened my eyes and played Elden Ring.'),
(3, 1, 5, 'This game had turned me into a little boy with no mommy into a single father raising 4 kids in a crappy apartment. 10/10 would recommend to any little boy needing to evolve to man hood.'),
(1, 2, 5, 'A labyrinth of joy that I don�t want to escape!'),
(4, 2, 5, 'Genuinely one of the best games Ive ever played in my life, and Ive been gaming for 25 years. The attention to detail that SGG put into this game is absolutely phenomenal.'),
(6, 2, 5, 'Great game couldnt put it down I played this on steam deck and runs really well, looking forward to playing hades 2 when im not so broke to buy it'),
(2, 3, 5, 'This game itches that city builder, colony sim scratch so well while being so DAM cute. I love it'),
(3, 3, 4, 'Not the most complex citybuilder/factory game hybrid imaginable, but very relaxing and just about any screenshot you take looks great. Normal difficulty is fairly well balanced, and everytime Ive lost it has been entirely my own fault. Scaling feels good, though I hope the devs latest focus on performance improvements keep up, as it can get a bit sluggish in the late-game.'),
(4, 3, 5, 'A cozy and addictive builder. Fun for hours as is. The devs seem to have a good direction, proven by consistently good patches. Community is nice and active, easily connected on discord.'),
(4, 4, 3, 'Its such a shameless clone that it even starts with a nature-themed first level and a narrow second one, just like in Vampire Survivors. Some of the weapons are almost identical too.'),
(5, 4, 4, 'Let me preface this by making it very clear that I dont know a single thing about vTubers or Hololive. But Ill be damned if this isnt a great game!'),
(6, 4, 5, 'Ffs, Kay... Let me give you money for this wonderful game!'),
(6, 5, 5, 'The coziest of cozy games. I always come back to it. The music has me break dancing or vibing in my room at 3am. 100/10'),
(2, 5, 5, 'Stardew Valley is hands down one of my all-time favorite games. the cozy music / the pixel art / the seasonal events... absolutely BEAUTIFUL! I enjoyed every single part of it from farming to mining.'),
(1, 5, 5, 'Addicting, one of the best indie games ever, there is so much content and if you manage to get bored you can find SO MANY mods to try. Definitely recommended.'),
(3, 6, 5, 'Black Myth: Wukong was my first real dive into the Soulslike genre, so I wasn�t sure what to expect going in. Right off the bat, the game hits hard � it�s polished, atmospheric, and sets a high bar in the early hours. It got me hooked enough to go for 100% achievements and see all the endings.'),
(1, 6, 5, '153 hours into the game and it still impresses me with each playthrough. cannot wait for the DLC and will never understand how Astrobot beat this game out for GOTY'),
(5, 6, 5, 'If you liked Elden Ring, youll probably love this game too once you get into it. Has some great cut-scenes and story along with the boss fighting action');