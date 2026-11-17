# E-Wallet Application

Aplikasi E-Wallet berbasis ASP.NET Core Web API dengan fitur pengelolaan pengguna, wallet, top up, transfer saldo, wallet mutasi, dan auditlog.

Aplikasi ini menggunakan Entity Framework Core sebagai ORM untuk mengelola komunikasi antara aplikasi dan SQL Server.

## 1. Fitur Aplikasi

### User Management
- Menampilkan daftar pengguna.
- Menampilkan detail pengguna.
- Menambahkan pengguna beserta wallet.
- Mengubah data pengguna.

### Wallet Management
- Menampilkan informasi wallet.
- Melihat saldo wallet.
- Mengambil detail wallet berdasarkan ID.

### Top Up Request
- Membuat permintaan top up.
- Memilih metode pembayaran.
- Mengelola status permintaan top up.
- Menambahkan saldo setelah top up dikonfirmasi.
- Mencatat perubahan saldo ke WalletMutations.

### Transfer
- Transfer saldo antar-wallet.
- Validasi wallet pengirim dan penerima.
- Validasi kecukupan saldo.
- Mengurangi saldo pengirim.
- Menambahkan saldo penerima.
- Mencatat transaksi dan mutasi saldo.
- Menjaga konsistensi data menggunakan database transaction.

### Transaction & Reporting
- Melihat riwayat transaksi.
- Melihat histori mutasi wallet.
- Menghitung total pemasukan dan pengeluaran.
- Mendukung pagination.

### Audit & Reliability
- Mencatat aktivitas melalui AuditLogs.
- Menangani kegagalan operasi dengan rollback.
- Mencegah lost update menggunakan RowVersion.

## 2. Entity Relationship Diagram (ERD)

Database menggunakan tujuh entitas utama untuk mengelola pengguna, wallet, transaksi, mutasi, metode pembayaran, permintaan top up, dan audit aktivitas.

```mermaid
erDiagram
    USERS {
        BIGINT Id PK
        NVARCHAR Username UK
        NVARCHAR FullName
        NVARCHAR Email UK
        NVARCHAR Phone UK
        NVARCHAR PasswordHash
        VARCHAR Status
        DATETIME2 CreatedAtUtc
        DATETIME2 UpdatedAtUtc
    }

    WALLETS {
        BIGINT Id PK
        BIGINT UserId FK,UK
        NVARCHAR WalletNumber UK
        DECIMAL Balance
        VARCHAR Status
        ROWVERSION RowVersion
        DATETIME2 CreatedAtUtc
        DATETIME2 UpdatedAtUtc
    }

    TRANSACTIONS {
        BIGINT Id PK
        NVARCHAR ReferenceNo UK
        BIGINT SenderWalletId FK
        BIGINT ReceiverWalletId FK
        VARCHAR Type
        DECIMAL Amount
        VARCHAR Status
        NVARCHAR Description
        DATETIME2 CreatedAtUtc
        DATETIME2 CompletedAtUtc
    }

    WALLET_MUTATIONS {
        BIGINT Id PK
        BIGINT TransactionId FK
        BIGINT WalletId FK
        VARCHAR Direction
        DECIMAL Amount
        DECIMAL BalanceBefore
        DECIMAL BalanceAfter
        DATETIME2 CreatedAtUtc
    }

    PAYMENT_METHODS {
        INT Id PK
        NVARCHAR Code UK
        NVARCHAR Name
        NVARCHAR ProviderName
        BIT IsActive
        DATETIME2 CreatedAtUtc
    }

    TOPUP_REQUESTS {
        BIGINT Id PK
        BIGINT TransactionId FK,UK
        BIGINT WalletId FK
        INT PaymentMethodId FK
        DECIMAL Amount
        VARCHAR Status
        NVARCHAR ProviderReference
        DATETIME2 CreatedAtUtc
        DATETIME2 ExpiresAtUtc
        DATETIME2 PaidAtUtc
    }

    AUDIT_LOGS {
        BIGINT Id PK
        BIGINT ActorUserId FK
        BIGINT TransactionId FK
        NVARCHAR Action
        NVARCHAR EntityType
        NVARCHAR EntityId
        VARCHAR Outcome
        NVARCHAR Description
        NVARCHAR CorrelationId
        DATETIME2 CreatedAtUtc
    }

    USERS ||--|| WALLETS : owns

    USERS |o--o{ AUDIT_LOGS : performs

    WALLETS |o--o{ TRANSACTIONS : sends
    WALLETS ||--o{ TRANSACTIONS : receives

    WALLETS ||--o{ WALLET_MUTATIONS : has
    TRANSACTIONS ||--o{ WALLET_MUTATIONS : produces

    WALLETS ||--o{ TOPUP_REQUESTS : requests


    PAYMENT_METHODS ||--o{ TOPUP_REQUESTS : used_for

    TRANSACTIONS ||--o| TOPUP_REQUESTS : topup_detail

    TRANSACTIONS |o--o{ AUDIT_LOGS : referenced_by
```


### Penjelasan Relasi

- **Users - Wallets:** Setiap pengguna memiliki wallet.
- **Wallets - Transactions:** Wallet dapat menjadi pengirim maupun penerima transaksi.
- **Wallets - WalletMutations:** Setiap wallet memiliki histori perubahan saldo.
- **Transactions - WalletMutations:** Transaksi menghasilkan catatan mutasi saldo.
- **Wallets - TopUpRequests:** Wallet dapat memiliki beberapa permintaan top up.
- **PaymentMethods - TopUpRequests:** Metode pembayaran digunakan dalam permintaan top up.
- **Users - AuditLogs:** Aktivitas pengguna dicatat dalam audit logs.

## 3. Database Migration

Project menggunakan Entity Framework Core Migration untuk mengelola perubahan struktur database.

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### Membuat Migration

Masuk ke folder project API:

```bash
cd Ewallet.Api
```

Buat migration pertama:

```bash
dotnet ef migrations add InitialCreate
```

Untuk perubahan struktur database berikutnya:

```bash
dotnet ef migrations add AddWalletMutations
```


### Menjalankan Migration

Terapkan migration ke SQL Server:

```bash
dotnet ef database update
```

### Melihat Daftar Migration

```bash
dotnet ef migrations list
```

## 4. Database Seeding

Database seeding digunakan untuk memasukkan data awal agar aplikasi dapat langsung diuji tanpa harus memasukkan seluruh data secara manual.

Seeding dapat digunakan untuk:
- Data pengguna awal.
- Data wallet awal.
- Data metode pembayaran.


```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString)
        .UseSeeding((context, _) =>
        {
            PaymentMethodSeedData.Seed(context);
            UserSeedData.Seed(context);
        })
        .UseAsyncSeeding(async (context, _, ct) =>
        {
            await PaymentMethodSeedData.SeedAsync(context, ct);
            await UserSeedData.SeedAsync(context,ct);
        });
});
```


## 5. Konfigurasi Database

Atur connection string pada program.cs.

```json
var connectionString = builder.Configuration.GetConnectionString("EwalletWeekly6") ?? throw new InvalidOperationException("connection belum diconfigurasi");
```

Contoh konfigurasi DbContext:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString)
```
