# 🚗 Driving & Vehicle License Department (DVLD) System

A robust, enterprise-level C# WinForms desktop application designed to fully automate and manage the core operations of a Driving & Vehicle Licensing Authority.

This project was built from scratch following strict software engineering principles, utilizing a decoupled **3-Tier Architecture** to ensure high scalability, security, and maintainability.

---

## 🌟 Overview & Key Highlights

The **DVLD System** handles the end-to-end lifecycle of driver licensing. From registering individuals and managing system users to running step-by-step driver test tracks, issuing local/international permits, and enforcing detention/release workflows.

### 🏗️ Architecture & Technical Highlights
- **Decoupled 3-Tier Architecture:** Complete separation between Presentation Layer (WinForms & Custom User Controls), Business Logic Layer (BLL), and Data Access Layer (DAL using ADO.NET).
- **Custom User Controls:** Heavy reusability of UI components (e.g., Person Card, Driver Info, Application Details, Filter Controls) to maintain clean code and DRY principles.
- **Relational Database Design:** Normalization, parameterized SQL queries, transactions, and robust data constraints executed on Microsoft SQL Server.
- **Business Rules Enforcement:** Strict tracking of test retakes, application prerequisites, expiration dates, and multi-status logic (New, Cancelled, Completed).

---

## 🛠️ Main Modules & Features

### 1. 👥 People & User Management
- **People Directory:** Comprehensive individual records with profile photo attachments, national IDs, contact info, and smart filtering.
- **System Users:** User account lifecycle with encrypted credentials, activity statuses, and role checks.

### 2. 📝 Applications & Testing Engine
- **New Local Driving License Applications:** Multi-class license support (e.g., Commercial, Non-Commercial, Motorcycle).
- **3-Stage Testing Workflow:**
  1. 👁️ **Vision Test**
  2. 📝 **Written Test**
  3. 🚗 **Street Test**
  *Includes automated locks preventing moving to the next test until previous stages are passed, along with test retake application handling.*

### 3. 💳 License Issuance & Operations
- **License Issuance:** Instant generation upon passing all 3 tests.
- **License Renewal & Replacements:** Workflows for expired licenses, lost permits, or damaged license replacements.
- **International Licenses:** Issuing international permits linked directly to valid local licenses with verification rules.

### 4. 🛑 License Detain & Release
- **Detain Management:** Locking licenses with fine amounts and detention reasons.
- **Release Management:** Releasing detained licenses after verifying fine payments and updating application logs.

---

## 🧰 Tech Stack & Tools

- **Language:** C# (.NET Framework)
- **UI Framework:** Windows Forms (WinForms)
- **Database:** Microsoft SQL Server
- **Data Access:** ADO.NET (Raw SQL Queries, Stored Procedures, DataTables, DataReaders)
- **Version Control:** Git & GitHub
- **IDE:** Visual Studio 2022

---

## 📂 Repository Structure

```text
DVLD_Project/
│
├── DB_Layer/                   #  Data Access Layer (DAL)
├── Driving License Management/ # Presentation Layer (WinForms UI & Business Logic Layer (BLL) & Custom User Controls)
└── DVLD-People-Images/         # Application media & person profile image storage
