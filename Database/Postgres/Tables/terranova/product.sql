CREATE TABLE terranova.product (
  id char(26) PRIMARY KEY,
  name varchar(100) NOT NULL,
  description text,
  price numeric(18, 2) NOT NULL,
  stock integer NOT NULL,
  sku varchar(50) NOT NULL UNIQUE,
  weight numeric(10,2),
  dimensions varchar(100),
  is_active boolean NOT NULL DEFAULT TRUE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz,
  is_deleted boolean NOT NULL DEFAULT FALSE
);