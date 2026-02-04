CREATE TABLE terranova.users (
  id char(26) PRIMARY KEY,
  first_name varchar(100) NOT NULL,
  last_name varchar(100) NOT NULL,
  phone_number varchar(20),
  birth_date timestamptz,
  gender char(1),
  password_hash varchar(255) NOT NULL,
  is_active boolean NOT NULL DEFAULT TRUE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  email_address varchar(255) NOT NULL UNIQUE,
  is_deleted boolean NOT NULL DEFAULT FALSE
);

-- Checks and Uniques
ALTER TABLE terranova.users ADD CONSTRAINT chk_users_email_address_format CHECK (email_address ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$');
ALTER TABLE terranova.users ADD CONSTRAINT chk_users_phone_number_format CHECK (phone_number IS NULL OR phone_number ~ '^\+?[0-9]{7,15}$');
ALTER TABLE terranova.users ADD CONSTRAINT chk_users_gender_valid CHECK (gender IS NULL OR gender IN ('M', 'F', 'O'));