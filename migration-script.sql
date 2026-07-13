IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);

CREATE TABLE [GymServices] (
    [ServiceID] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    [DurationInDays] int NOT NULL DEFAULT 0,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    [AllowedSessionsCount] int NOT NULL,
    CONSTRAINT [PK_GymServices] PRIMARY KEY ([ServiceID])
);

CREATE TABLE [Suppliers] (
    [SupplierID] int NOT NULL IDENTITY,
    [CompanyName] nvarchar(100) NOT NULL,
    [SupplierPhone] nvarchar(20) NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Suppliers] PRIMARY KEY ([SupplierID])
);

CREATE TABLE [Users] (
    [UserID] int NOT NULL IDENTITY,
    [FullName] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [Status] int NOT NULL,
    [JoinDate] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserID])
);

CREATE TABLE [Products] (
    [ProductID] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Barcode] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [CurrentSellPrice] decimal(18,2) NOT NULL,
    [ReorderLevel] int NOT NULL,
    [CategoryId] int NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    [SupplierID] int NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([ProductID]),
    CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Products_Suppliers_SupplierID] FOREIGN KEY ([SupplierID]) REFERENCES [Suppliers] ([SupplierID]) ON DELETE SET NULL
);

CREATE TABLE [AuditLogs] (
    [Id] int NOT NULL IDENTITY,
    [EntityName] nvarchar(max) NOT NULL,
    [Action] nvarchar(max) NOT NULL,
    [OldValue] nvarchar(max) NULL,
    [NewValue] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [EntityPrimaryKey] int NOT NULL,
    [UserId] int NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AuditLogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserID])
);

CREATE TABLE [Carts] (
    [CartID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Carts] PRIMARY KEY ([CartID]),
    CONSTRAINT [FK_Carts_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE CASCADE
);

CREATE TABLE [InventoryTransactions] (
    [TransactionID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [TransactionDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [ReferenceNumber] nvarchar(50) NOT NULL,
    [Notes] nvarchar(500) NOT NULL,
    CONSTRAINT [PK_InventoryTransactions] PRIMARY KEY ([TransactionID]),
    CONSTRAINT [FK_InventoryTransactions_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION
);

CREATE TABLE [Invoices] (
    [InvoiceID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [IssueDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [TotalAmount] decimal(18,2) NOT NULL,
    [InvoiceStatus] nvarchar(max) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY ([InvoiceID]),
    CONSTRAINT [FK_Invoices_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION
);

CREATE TABLE [MemberProfiles] (
    [MemberProfileId] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [QRCodeData] nvarchar(max) NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_MemberProfiles] PRIMARY KEY ([MemberProfileId]),
    CONSTRAINT [FK_MemberProfiles_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE CASCADE
);

CREATE TABLE [Notifications] (
    [NotificationID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [Content] nvarchar(500) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Type] int NOT NULL,
    [IsRead] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([NotificationID]),
    CONSTRAINT [FK_Notifications_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE CASCADE
);

CREATE TABLE [Trainers] (
    [TrainerID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [Specialization] nvarchar(100) NOT NULL,
    [Bio] nvarchar(500) NOT NULL,
    [WorkingHours] nvarchar(100) NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Trainers] PRIMARY KEY ([TrainerID]),
    CONSTRAINT [FK_Trainers_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE CASCADE
);

CREATE TABLE [UserRoles] (
    [RoleID] int NOT NULL IDENTITY,
    [Role] nvarchar(max) NOT NULL,
    [UserID] int NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([RoleID]),
    CONSTRAINT [FK_UserRoles_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE CASCADE
);

CREATE TABLE [Inventories] (
    [Id] int NOT NULL IDENTITY,
    [ExpiryDate] datetime2 NULL,
    [DateAdded] datetime2 NOT NULL,
    [Quantity] int NOT NULL,
    [CostPrice] decimal(18,2) NOT NULL,
    [ProductId] int NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Inventories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Inventories_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductID]) ON DELETE NO ACTION
);

CREATE TABLE [CartItems] (
    [CartItemID] int NOT NULL IDENTITY,
    [CartID] int NOT NULL,
    [ProductID] int NOT NULL,
    [Quantity] int NOT NULL DEFAULT 1,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_CartItems] PRIMARY KEY ([CartItemID]),
    CONSTRAINT [FK_CartItems_Carts_CartID] FOREIGN KEY ([CartID]) REFERENCES [Carts] ([CartID]) ON DELETE CASCADE,
    CONSTRAINT [FK_CartItems_Products_ProductID] FOREIGN KEY ([ProductID]) REFERENCES [Products] ([ProductID]) ON DELETE NO ACTION
);

CREATE TABLE [InventoryTransactionsItems] (
    [TransactionItemID] int NOT NULL IDENTITY,
    [TransactionID] int NOT NULL,
    [ProductID] int NULL,
    [ProductName] nvarchar(max) NOT NULL,
    [Quantity] int NOT NULL,
    [UnitCost] decimal(18,2) NOT NULL,
    [BatchNumber] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_InventoryTransactionsItems] PRIMARY KEY ([TransactionItemID]),
    CONSTRAINT [FK_InventoryTransactionsItems_InventoryTransactions_TransactionID] FOREIGN KEY ([TransactionID]) REFERENCES [InventoryTransactions] ([TransactionID]) ON DELETE CASCADE,
    CONSTRAINT [FK_InventoryTransactionsItems_Products_ProductID] FOREIGN KEY ([ProductID]) REFERENCES [Products] ([ProductID]) ON DELETE SET NULL
);

CREATE TABLE [Payments] (
    [PaymentID] int NOT NULL IDENTITY,
    [InvoiceID] int NOT NULL,
    [UserId] int NOT NULL,
    [AmountPaid] decimal(18,2) NOT NULL,
    [PaymentDate] datetime2 NOT NULL,
    [PaymentMethod] nvarchar(max) NOT NULL,
    [TransactionReference] nvarchar(100) NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([PaymentID]),
    CONSTRAINT [FK_Payments_Invoices_InvoiceID] FOREIGN KEY ([InvoiceID]) REFERENCES [Invoices] ([InvoiceID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Payments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserID]) ON DELETE NO ACTION
);

CREATE TABLE [Classes] (
    [ClassID] int NOT NULL IDENTITY,
    [TrainerID] int NOT NULL,
    [ClassName] nvarchar(100) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Capacity] int NOT NULL,
    [NumberOfSessions] int NOT NULL,
    [Status] int NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Classes] PRIMARY KEY ([ClassID]),
    CONSTRAINT [FK_Classes_Trainers_TrainerID] FOREIGN KEY ([TrainerID]) REFERENCES [Trainers] ([TrainerID]) ON DELETE NO ACTION
);

CREATE TABLE [PrivateSessions] (
    [PrivateSessionID] int NOT NULL IDENTITY,
    [TrainerID] int NOT NULL,
    [MemberUserId] int NOT NULL,
    [SessionDate] datetime2 NOT NULL,
    [StartTime] time NOT NULL,
    [EndTime] time NOT NULL,
    [Status] int NOT NULL,
    [Notes] nvarchar(500) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_PrivateSessions] PRIMARY KEY ([PrivateSessionID]),
    CONSTRAINT [FK_PrivateSessions_MemberProfiles_MemberUserId] FOREIGN KEY ([MemberUserId]) REFERENCES [MemberProfiles] ([MemberProfileId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PrivateSessions_Trainers_TrainerID] FOREIGN KEY ([TrainerID]) REFERENCES [Trainers] ([TrainerID]) ON DELETE NO ACTION
);

CREATE TABLE [TrainerWorkingHours] (
    [Id] int NOT NULL IDENTITY,
    [TrainerID] int NOT NULL,
    [Day] int NOT NULL,
    [StartTime] time NOT NULL,
    [EndTime] time NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_TrainerWorkingHours] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TrainerWorkingHours_Trainers_TrainerID] FOREIGN KEY ([TrainerID]) REFERENCES [Trainers] ([TrainerID]) ON DELETE CASCADE
);

CREATE TABLE [Bookings] (
    [BookingID] int NOT NULL IDENTITY,
    [ClassID] int NULL,
    [GymServiceId] int NULL,
    [MemberUserId] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Bookings] PRIMARY KEY ([BookingID]),
    CONSTRAINT [CK_Booking_ClassOrService_Exclusive] CHECK ((ClassID IS NOT NULL AND GymServiceId IS NULL) OR (ClassID IS NULL AND GymServiceId IS NOT NULL)),
    CONSTRAINT [FK_Bookings_Classes_ClassID] FOREIGN KEY ([ClassID]) REFERENCES [Classes] ([ClassID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Bookings_GymServices_GymServiceId] FOREIGN KEY ([GymServiceId]) REFERENCES [GymServices] ([ServiceID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Bookings_MemberProfiles_MemberUserId] FOREIGN KEY ([MemberUserId]) REFERENCES [MemberProfiles] ([MemberProfileId]) ON DELETE NO ACTION
);

CREATE TABLE [ClassSchedule] (
    [Id] int NOT NULL IDENTITY,
    [ClassID] int NOT NULL,
    [Day] int NOT NULL,
    [StartTime] time NOT NULL,
    [EndTime] time NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_ClassSchedule] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClassSchedule_Classes_ClassID] FOREIGN KEY ([ClassID]) REFERENCES [Classes] ([ClassID]) ON DELETE CASCADE
);

CREATE TABLE [InvoiceItems] (
    [InvoiceItemID] int NOT NULL IDENTITY,
    [InvoiceID] int NOT NULL,
    [ItemType] nvarchar(max) NOT NULL,
    [ProductID] int NULL,
    [ServiceID] int NULL,
    [ClassID] int NULL,
    [ItemName] nvarchar(200) NOT NULL,
    [Quantity] int NOT NULL DEFAULT 1,
    [LineTotal] decimal(18,2) NOT NULL,
    [SellPrice] decimal(18,2) NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_InvoiceItems] PRIMARY KEY ([InvoiceItemID]),
    CONSTRAINT [CK_InvoiceItem_TypeAllowed] CHECK ((CASE WHEN ProductID IS NOT NULL THEN 1 ELSE 0 END +  CASE WHEN ServiceID IS NOT NULL THEN 1 ELSE 0 END +  CASE WHEN ClassID IS NOT NULL THEN 1 ELSE 0 END) = 1),
    CONSTRAINT [FK_InvoiceItems_Classes_ClassID] FOREIGN KEY ([ClassID]) REFERENCES [Classes] ([ClassID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_InvoiceItems_GymServices_ServiceID] FOREIGN KEY ([ServiceID]) REFERENCES [GymServices] ([ServiceID]) ON DELETE SET NULL,
    CONSTRAINT [FK_InvoiceItems_Invoices_InvoiceID] FOREIGN KEY ([InvoiceID]) REFERENCES [Invoices] ([InvoiceID]) ON DELETE CASCADE,
    CONSTRAINT [FK_InvoiceItems_Products_ProductID] FOREIGN KEY ([ProductID]) REFERENCES [Products] ([ProductID]) ON DELETE SET NULL
);

CREATE TABLE [Memberships] (
    [MembershipID] int NOT NULL IDENTITY,
    [MemberProfileId] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [FreezeStartDate] datetime2 NULL,
    [FreezeEndDate] datetime2 NULL,
    [IsAutoRenew] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [GymServiceId] int NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    [ClassID] int NULL,
    [RemainingSessions] int NULL,
    [UserID] int NULL,
    CONSTRAINT [PK_Memberships] PRIMARY KEY ([MembershipID]),
    CONSTRAINT [FK_Memberships_Classes_ClassID] FOREIGN KEY ([ClassID]) REFERENCES [Classes] ([ClassID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Memberships_GymServices_GymServiceId] FOREIGN KEY ([GymServiceId]) REFERENCES [GymServices] ([ServiceID]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Memberships_MemberProfiles_MemberProfileId] FOREIGN KEY ([MemberProfileId]) REFERENCES [MemberProfiles] ([MemberProfileId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Memberships_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
);

CREATE TABLE [Attendances] (
    [AttendanceID] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [MembershipID] int NULL,
    [Type] int NOT NULL,
    [CheckInTime] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    [ClassID] int NULL,
    CONSTRAINT [PK_Attendances] PRIMARY KEY ([AttendanceID]),
    CONSTRAINT [FK_Attendances_Classes_ClassID] FOREIGN KEY ([ClassID]) REFERENCES [Classes] ([ClassID]),
    CONSTRAINT [FK_Attendances_MemberProfiles_UserId] FOREIGN KEY ([UserId]) REFERENCES [MemberProfiles] ([MemberProfileId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Attendances_Memberships_MembershipID] FOREIGN KEY ([MembershipID]) REFERENCES [Memberships] ([MembershipID]) ON DELETE SET NULL
);

CREATE INDEX [IX_Attendances_ClassID] ON [Attendances] ([ClassID]);

CREATE INDEX [IX_Attendances_MembershipID] ON [Attendances] ([MembershipID]);

CREATE INDEX [IX_Attendances_UserId] ON [Attendances] ([UserId]);

CREATE INDEX [IX_AuditLogs_UserId] ON [AuditLogs] ([UserId]);

CREATE INDEX [IX_Bookings_ClassID] ON [Bookings] ([ClassID]);

CREATE INDEX [IX_Bookings_GymServiceId] ON [Bookings] ([GymServiceId]);

CREATE INDEX [IX_Bookings_MemberUserId] ON [Bookings] ([MemberUserId]);

CREATE INDEX [IX_CartItems_CartID] ON [CartItems] ([CartID]);

CREATE INDEX [IX_CartItems_ProductID] ON [CartItems] ([ProductID]);

CREATE UNIQUE INDEX [IX_Carts_UserID] ON [Carts] ([UserID]);

CREATE INDEX [IX_Classes_TrainerID] ON [Classes] ([TrainerID]);

CREATE INDEX [IX_ClassSchedule_ClassID] ON [ClassSchedule] ([ClassID]);

CREATE INDEX [IX_Inventories_ProductId] ON [Inventories] ([ProductId]);

CREATE INDEX [IX_InventoryTransactions_UserID] ON [InventoryTransactions] ([UserID]);

CREATE INDEX [IX_InventoryTransactionsItems_ProductID] ON [InventoryTransactionsItems] ([ProductID]);

CREATE INDEX [IX_InventoryTransactionsItems_TransactionID] ON [InventoryTransactionsItems] ([TransactionID]);

CREATE INDEX [IX_InvoiceItems_ClassID] ON [InvoiceItems] ([ClassID]);

CREATE INDEX [IX_InvoiceItems_InvoiceID] ON [InvoiceItems] ([InvoiceID]);

CREATE INDEX [IX_InvoiceItems_ProductID] ON [InvoiceItems] ([ProductID]);

CREATE INDEX [IX_InvoiceItems_ServiceID] ON [InvoiceItems] ([ServiceID]);

CREATE INDEX [IX_Invoices_UserID] ON [Invoices] ([UserID]);

CREATE UNIQUE INDEX [IX_MemberProfiles_UserID] ON [MemberProfiles] ([UserID]);

CREATE INDEX [IX_Memberships_ClassID] ON [Memberships] ([ClassID]);

CREATE INDEX [IX_Memberships_GymServiceId] ON [Memberships] ([GymServiceId]);

CREATE INDEX [IX_Memberships_MemberProfileId] ON [Memberships] ([MemberProfileId]);

CREATE INDEX [IX_Memberships_UserID] ON [Memberships] ([UserID]);

CREATE INDEX [IX_Notifications_UserID] ON [Notifications] ([UserID]);

CREATE INDEX [IX_Payments_InvoiceID] ON [Payments] ([InvoiceID]);

CREATE INDEX [IX_Payments_UserId] ON [Payments] ([UserId]);

CREATE INDEX [IX_PrivateSessions_MemberUserId] ON [PrivateSessions] ([MemberUserId]);

CREATE INDEX [IX_PrivateSessions_TrainerID] ON [PrivateSessions] ([TrainerID]);

CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);

CREATE INDEX [IX_Products_SupplierID] ON [Products] ([SupplierID]);

CREATE UNIQUE INDEX [IX_Trainers_UserID] ON [Trainers] ([UserID]);

CREATE INDEX [IX_TrainerWorkingHours_TrainerID] ON [TrainerWorkingHours] ([TrainerID]);

CREATE INDEX [IX_UserRoles_UserID] ON [UserRoles] ([UserID]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260711181038_initial', N'9.0.0');

ALTER TABLE [Payments] ADD [GatewayResponse] nvarchar(max) NULL;

ALTER TABLE [Memberships] ADD [AllowedVisits] int NULL;

ALTER TABLE [Memberships] ADD [ConsumedVisits] int NOT NULL DEFAULT 0;

ALTER TABLE [Memberships] ADD [InvoiceID] int NULL;

ALTER TABLE [Invoices] ADD [DiscountAmount] decimal(18,2) NOT NULL DEFAULT 0.0;

ALTER TABLE [Invoices] ADD [DueDate] datetime2 NULL;

ALTER TABLE [Invoices] ADD [SubTotal] decimal(18,2) NOT NULL DEFAULT 0.0;

ALTER TABLE [Invoices] ADD [TaxAmount] decimal(18,2) NOT NULL DEFAULT 0.0;

ALTER TABLE [InvoiceItems] ADD [Discount] decimal(18,2) NOT NULL DEFAULT 0.0;

CREATE INDEX [IX_Memberships_InvoiceID] ON [Memberships] ([InvoiceID]);

ALTER TABLE [Memberships] ADD CONSTRAINT [FK_Memberships_Invoices_InvoiceID] FOREIGN KEY ([InvoiceID]) REFERENCES [Invoices] ([InvoiceID]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260711210702_payment', N'9.0.0');

COMMIT;
GO

