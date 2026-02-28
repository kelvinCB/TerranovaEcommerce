BEGIN;

-- 1. Esquema/Schema
CREATE SCHEMA IF NOT EXISTS terranova;
SET search_path TO terranova;


-- 2. Tablas/Tables

CREATE TABLE terranova.cart (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL UNIQUE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW()
);

CREATE TABLE terranova.cart_item (
  id char(26) PRIMARY KEY,
  cart_id char(26) NOT NULL,
  product_id char(26) NOT NULL,
  quantity integer NOT NULL,
  is_selected boolean NOT NULL DEFAULT FALSE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW()
);

CREATE TABLE terranova.category (
  id char(26) PRIMARY KEY,
  name varchar(100) NOT NULL UNIQUE,
  description text,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.invoice (
  id char(26) PRIMARY KEY,
  invoice_number varchar(50) NOT NULL UNIQUE,
  order_id char(26) NOT NULL UNIQUE,
  invoice_status_id char(26) NOT NULL,
  total_amount numeric(18, 2) NOT NULL,
  shipping_amount numeric(18, 2) NOT NULL,
  invoice_date timestamptz NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.invoice_item (
  id char(26) PRIMARY KEY,
  invoice_id char(26) NOT NULL,
  product_id char(26) NOT NULL,
  quantity integer NOT NULL,
  unit_price numeric(18, 2) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.invoice_status (
  id char(26) PRIMARY KEY,
  name varchar(50) NOT NULL UNIQUE,
  description varchar(255),
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.orders (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  order_status_id char(26) NOT NULL,
  total_amount numeric(18, 2) NOT NULL,
  shipping_amount numeric(18, 2) NOT NULL,
  shipping_address_id char(26) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.order_item (
  id char(26) PRIMARY KEY,
  order_id char(26) NOT NULL,
  product_id char(26) NOT NULL,
  quantity integer NOT NULL,
  unit_price numeric(18, 2) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.order_status (
  id char(26) PRIMARY KEY,
  name varchar(50) NOT NULL UNIQUE,
  description varchar(255),
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.payment (
  id char(26) PRIMARY KEY,
  order_id char(26) NOT NULL,
  payment_method_id char(26) NOT NULL,
  payment_status_id char(26) NOT NULL,
  amount numeric(18, 2) NOT NULL,
  transaction_reference varchar(100),
  paid_at timestamptz,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.payment_method (
  id char(26) PRIMARY KEY,
  name varchar(50) NOT NULL UNIQUE,
  description varchar(255),
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.payment_status (
  id char(26) PRIMARY KEY,
  name varchar(50) NOT NULL UNIQUE,
  description varchar(255),
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.payment_webhook_log (
  id char(26) PRIMARY KEY,
  payment_id char(26) NOT NULL,
  provider varchar(50) NOT NULL,
  event_type varchar(100) NOT NULL,
  payload jsonb NOT NULL,
  received_at timestamptz NOT NULL,
  processed_at timestamptz,
  status varchar(50) NOT NULL,
  error_message text,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW()
);

CREATE TABLE terranova.product (
  id char(26) PRIMARY KEY,
  name varchar(100) NOT NULL,
  description text,
  price numeric(18, 2) NOT NULL,
  stock integer NOT NULL,
  sku varchar(50) NOT NULL UNIQUE,
  weight numeric(10,2),
  dimensions varchar(100),
  is_active boolean NOT NULL DEFAULT FALSE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.product_image (
  id char(26) PRIMARY KEY,
  product_id char(26) NOT NULL,
  image_url varchar(500) NOT NULL,
  is_main boolean NOT NULL DEFAULT FALSE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.product_sub_category (
  id char(26),
  product_id char(26) NOT NULL,
  sub_category_id char(26) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  PRIMARY KEY(product_id, sub_category_id)
);

CREATE TABLE terranova.product_variant (
  id char(26) PRIMARY KEY,
  product_id char(26) NOT NULL,
  name varchar(100) NOT NULL,
  value varchar(100) NOT NULL,
  price_adjustment numeric(18, 2) NOT NULL,
  stock integer NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.product_variant_image (
  id char(26) PRIMARY KEY,
  product_variant_id char(26) NOT NULL,
  image_url varchar(500) NOT NULL,
  is_main boolean NOT NULL DEFAULT FALSE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.shipping_address (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  address1 varchar(200) NOT NULL,
  address2 varchar(200),
  city varchar(150) NOT NULL,
  state varchar(50),
  country varchar(50) NOT NULL,
  postal_code varchar(20) NOT NULL,
  contact_phone varchar(20),
  is_default boolean NOT NULL DEFAULT FALSE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.sub_category (
  id char(26) PRIMARY KEY,
  name varchar(100) NOT NULL,
  description text,
  category_id char(26) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

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

CREATE TABLE terranova.wish_list (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL UNIQUE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

CREATE TABLE terranova.wish_list_item (
  id char(26) PRIMARY KEY,
  wish_list_id char(26) NOT NULL,
  product_id char(26) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

-- Auth/JWT tables
CREATE TABLE terranova.roles (
  id char(26) PRIMARY KEY,
  name varchar(50) NOT NULL UNIQUE,
  description varchar(200),
  created_at timestamptz NOT NULL DEFAULT NOW()
);

CREATE TABLE terranova.user_roles (
  user_id char(26) NOT NULL,
  role_id char(26) NOT NULL,
  assigned_at timestamptz NOT NULL DEFAULT now(),
  PRIMARY KEY (user_id, role_id)
);

-- Códigos de verificación (email, sms, reset password, 2fa, etc.)
CREATE TABLE terranova.user_verifications (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  purpose varchar(30) NOT NULL,            -- email_verify, reset_password, mfa
  code_hash varchar(255) NOT NULL,
  expires_at timestamptz NOT NULL,
  consumed_at timestamptz,
  created_at timestamptz NOT NULL DEFAULT now()
);

-- Refresh tokens para JWT (rotación/revocación)
CREATE TABLE terranova.refresh_tokens (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  token_hash varchar(255) NOT NULL,        -- hash del refresh token
  jti varchar(100),                        -- JWT ID (opcional)
  expires_at timestamptz NOT NULL,
  is_revoked boolean NOT NULL DEFAULT FALSE,
  revoked_at timestamptz,
  replaced_by_token_id char(26),
  created_at timestamptz NOT NULL DEFAULT now(),
  user_agent varchar(500),
  ip_address varchar(45)
);


-- 3. Indices/Indexes

-- refresh_tokens
CREATE INDEX IF NOT EXISTS ix_refresh_tokens_user_id ON terranova.refresh_tokens(user_id);

-- user_verifications
CREATE INDEX IF NOT EXISTS ix_user_verifications_user_id ON terranova.user_verifications(user_id);

-- user_verifications
CREATE INDEX IF NOT EXISTS ix_user_verifications_expires_at ON terranova.user_verifications(expires_at);

-- cart
CREATE INDEX IF NOT EXISTS ix_cart_user_id ON terranova.cart(user_id);

-- cart_item
CREATE INDEX IF NOT EXISTS ix_cart_item_cart_id ON terranova.cart_item(cart_id);
CREATE INDEX IF NOT EXISTS ix_cart_item_product_id ON terranova.cart_item(product_id);

-- product_image / variants
CREATE INDEX IF NOT EXISTS ix_product_image_product_id ON terranova.product_image(product_id);
CREATE INDEX IF NOT EXISTS ix_product_variant_product_id ON terranova.product_variant(product_id);
CREATE INDEX IF NOT EXISTS ix_product_variant_image_variant_id ON terranova.product_variant_image(product_variant_id);

-- sub_category
CREATE INDEX IF NOT EXISTS ix_sub_category_category_id ON terranova.sub_category(category_id);

-- product_sub_category (tabla puente)
CREATE INDEX IF NOT EXISTS ix_product_sub_category_product_id ON terranova.product_sub_category(product_id);
CREATE INDEX IF NOT EXISTS ix_product_sub_category_sub_category_id ON terranova.product_sub_category(sub_category_id);

-- orders / order_item
CREATE INDEX IF NOT EXISTS ix_orders_user_id ON terranova.orders(user_id);
CREATE INDEX IF NOT EXISTS ix_orders_order_status_id ON terranova.orders(order_status_id);
CREATE INDEX IF NOT EXISTS ix_orders_shipping_address_id ON terranova.orders(shipping_address_id);

CREATE INDEX IF NOT EXISTS ix_order_item_order_id ON terranova.order_item(order_id);
CREATE INDEX IF NOT EXISTS ix_order_item_product_id ON terranova.order_item(product_id);

-- invoice / invoice_item
CREATE INDEX IF NOT EXISTS ix_invoice_order_id ON terranova.invoice(order_id);
CREATE INDEX IF NOT EXISTS ix_invoice_invoice_status_id ON terranova.invoice(invoice_status_id);

CREATE INDEX IF NOT EXISTS ix_invoice_item_invoice_id ON terranova.invoice_item(invoice_id);
CREATE INDEX IF NOT EXISTS ix_invoice_item_product_id ON terranova.invoice_item(product_id);

-- payment / webhook
CREATE INDEX IF NOT EXISTS ix_payment_order_id ON terranova.payment(order_id);
CREATE INDEX IF NOT EXISTS ix_payment_payment_method_id ON terranova.payment(payment_method_id);
CREATE INDEX IF NOT EXISTS ix_payment_payment_status_id ON terranova.payment(payment_status_id);

CREATE INDEX IF NOT EXISTS ix_payment_webhook_log_payment_id ON terranova.payment_webhook_log(payment_id);

-- shipping_address / wishlist
CREATE INDEX IF NOT EXISTS ix_shipping_address_user_id ON terranova.shipping_address(user_id);
CREATE INDEX IF NOT EXISTS ix_wish_list_user_id ON terranova.wish_list(user_id);
CREATE INDEX IF NOT EXISTS ix_wish_list_item_wish_list_id ON terranova.wish_list_item(wish_list_id);
CREATE INDEX IF NOT EXISTS ix_wish_list_item_product_id ON terranova.wish_list_item(product_id);


-- 4. LLaves foraneas/Foreign keys

-- cart
ALTER TABLE terranova.cart ADD CONSTRAINT fk_cart_user FOREIGN KEY (user_id) REFERENCES terranova.users (id);

-- cart_item
ALTER TABLE terranova.cart_item ADD CONSTRAINT fk_cart_item_cart FOREIGN KEY (cart_id) REFERENCES terranova.cart (id);
ALTER TABLE terranova.cart_item ADD CONSTRAINT fk_cart_item_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);

-- invoice
ALTER TABLE terranova.invoice ADD CONSTRAINT fk_invoice_invoice_status FOREIGN KEY (invoice_status_id) REFERENCES terranova.invoice_status (id);
ALTER TABLE terranova.invoice ADD CONSTRAINT fk_invoice_order FOREIGN KEY (order_id) REFERENCES terranova.orders (id);

-- invoice_item
ALTER TABLE terranova.invoice_item ADD CONSTRAINT fk_invoice_item_invoice FOREIGN KEY (invoice_id) REFERENCES terranova.invoice (id);
ALTER TABLE terranova.invoice_item ADD CONSTRAINT fk_invoice_item_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);

-- orders
ALTER TABLE terranova.orders ADD CONSTRAINT fk_order_order_status FOREIGN KEY (order_status_id) REFERENCES terranova.order_status (id);
ALTER TABLE terranova.orders ADD CONSTRAINT fk_order_shipping_address FOREIGN KEY (shipping_address_id) REFERENCES terranova.shipping_address (id);
ALTER TABLE terranova.orders ADD CONSTRAINT fk_order_user FOREIGN KEY (user_id) REFERENCES terranova.users (id);

-- order_item
ALTER TABLE terranova.order_item ADD CONSTRAINT fk_order_item_order FOREIGN KEY (order_id) REFERENCES terranova.orders (id);
ALTER TABLE terranova.order_item ADD CONSTRAINT fk_order_item_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);

-- payment
ALTER TABLE terranova.payment ADD CONSTRAINT fk_payment_order FOREIGN KEY (order_id) REFERENCES terranova.orders (id);
ALTER TABLE terranova.payment ADD CONSTRAINT fk_payment_payment_method FOREIGN KEY (payment_method_id) REFERENCES terranova.payment_method (id);
ALTER TABLE terranova.payment ADD CONSTRAINT fk_payment_payment_status FOREIGN KEY (payment_status_id) REFERENCES terranova.payment_status (id);

-- payment_webhook_log
ALTER TABLE terranova.payment_webhook_log ADD CONSTRAINT fk_payment_webhook_log_payment FOREIGN KEY (payment_id) REFERENCES terranova.payment (id);

-- product_image
ALTER TABLE terranova.product_image ADD CONSTRAINT fk_product_image_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);

-- product_sub_category
ALTER TABLE terranova.product_sub_category ADD CONSTRAINT fk_product_sub_category_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);
ALTER TABLE terranova.product_sub_category ADD CONSTRAINT fk_product_sub_category_sub_category FOREIGN KEY (sub_category_id) REFERENCES terranova.sub_category (id);

-- product_variant
ALTER TABLE terranova.product_variant ADD CONSTRAINT fk_product_variant_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);

-- product_variant_image
ALTER TABLE terranova.product_variant_image ADD CONSTRAINT fk_product_variant_image_product_variant FOREIGN KEY (product_variant_id) REFERENCES terranova.product_variant (id);

-- shipping_address
ALTER TABLE terranova.shipping_address ADD CONSTRAINT fk_shipping_address_user FOREIGN KEY (user_id) REFERENCES terranova.users (id);

-- sub_category
ALTER TABLE terranova.sub_category ADD CONSTRAINT fk_sub_category_category FOREIGN KEY (category_id) REFERENCES terranova.category (id);

-- wish_list
ALTER TABLE terranova.wish_list ADD CONSTRAINT fk_wish_list_user FOREIGN KEY (user_id) REFERENCES terranova.users (id);

-- wish_list_item
ALTER TABLE terranova.wish_list_item ADD CONSTRAINT fk_wish_list_item_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);
ALTER TABLE terranova.wish_list_item ADD CONSTRAINT fk_wish_list_item_wish_list FOREIGN KEY (wish_list_id) REFERENCES terranova.wish_list (id);

-- user_roles
ALTER TABLE terranova.user_roles ADD CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) REFERENCES terranova.users(id);
ALTER TABLE terranova.user_roles ADD CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id) REFERENCES terranova.roles(id);

-- user_verifications
ALTER TABLE terranova.user_verifications ADD CONSTRAINT fk_user_verifications_user FOREIGN KEY (user_id) REFERENCES terranova.users(id);

-- refresh_tokens
ALTER TABLE terranova.refresh_tokens ADD CONSTRAINT fk_refresh_tokens_user FOREIGN KEY (user_id) REFERENCES terranova.users(id);
ALTER TABLE terranova.refresh_tokens ADD CONSTRAINT fk_refresh_tokens_replaced FOREIGN KEY (replaced_by_token_id) REFERENCES terranova.refresh_tokens(id);

-- 5. Condiciones/Constraints (checks and uniques)

-- product_image
CREATE UNIQUE INDEX IF NOT EXISTS only_one_main_product_image ON terranova.product_image(product_id) WHERE is_main = TRUE;

-- product_variant_image
CREATE UNIQUE INDEX IF NOT EXISTS only_one_main_product_variant_image ON terranova.product_variant_image(product_variant_id) WHERE is_main = TRUE;

-- shipping_address
CREATE UNIQUE INDEX IF NOT EXISTS only_one_default_shipping_address ON terranova.shipping_address(user_id) WHERE is_default = TRUE;

ALTER TABLE terranova.shipping_address DROP CONSTRAINT IF EXISTS chk_shipping_address_postal_code_not_empty;
ALTER TABLE terranova.shipping_address ADD CONSTRAINT chk_shipping_address_postal_code_not_empty CHECK (postal_code <> '');

-- wish_list
CREATE UNIQUE INDEX IF NOT EXISTS uq_wish_list_item_active ON terranova.wish_list_item (wish_list_id, product_id) WHERE is_deleted = FALSE;

-- cart_item
ALTER TABLE terranova.cart_item DROP CONSTRAINT IF EXISTS chk_cart_item_quantity_positive;
ALTER TABLE terranova.cart_item ADD CONSTRAINT chk_cart_item_quantity_positive CHECK (quantity > 0);

ALTER TABLE terranova.cart_item DROP CONSTRAINT IF EXISTS uq_cart_item_cart_product;
ALTER TABLE terranova.cart_item ADD CONSTRAINT uq_cart_item_cart_product UNIQUE (cart_id, product_id);

-- invoice
ALTER TABLE terranova.invoice DROP CONSTRAINT IF EXISTS chk_invoice_total_amount_positive;
ALTER TABLE terranova.invoice ADD CONSTRAINT chk_invoice_total_amount_positive CHECK (total_amount >= 0);

ALTER TABLE terranova.invoice DROP CONSTRAINT IF EXISTS chk_invoice_shipping_amount_positive;
ALTER TABLE terranova.invoice ADD CONSTRAINT chk_invoice_shipping_amount_positive CHECK (shipping_amount >= 0);

-- invoice_item
ALTER TABLE terranova.invoice_item DROP CONSTRAINT IF EXISTS uq_invoice_item_invoice_product;
ALTER TABLE terranova.invoice_item ADD CONSTRAINT uq_invoice_item_invoice_product UNIQUE (invoice_id, product_id);

ALTER TABLE terranova.invoice_item DROP CONSTRAINT IF EXISTS chk_invoice_item_quantity_positive;
ALTER TABLE terranova.invoice_item ADD CONSTRAINT chk_invoice_item_quantity_positive CHECK (quantity > 0);

ALTER TABLE terranova.invoice_item DROP CONSTRAINT IF EXISTS chk_invoice_item_unit_price_positive;
ALTER TABLE terranova.invoice_item ADD CONSTRAINT chk_invoice_item_unit_price_positive CHECK (unit_price >= 0);

-- orders
ALTER TABLE terranova.orders DROP CONSTRAINT IF EXISTS chk_orders_total_amount_positive;
ALTER TABLE terranova.orders ADD CONSTRAINT chk_orders_total_amount_positive CHECK (total_amount >= 0);

ALTER TABLE terranova.orders DROP CONSTRAINT IF EXISTS chk_orders_shipping_amount_positive;
ALTER TABLE terranova.orders ADD CONSTRAINT chk_orders_shipping_amount_positive CHECK (shipping_amount >= 0);

-- order_item
ALTER TABLE terranova.order_item DROP CONSTRAINT IF EXISTS uq_order_item_order_product;
ALTER TABLE terranova.order_item ADD CONSTRAINT uq_order_item_order_product UNIQUE (order_id, product_id);

ALTER TABLE terranova.order_item DROP CONSTRAINT IF EXISTS chk_order_item_quantity_positive;
ALTER TABLE terranova.order_item ADD CONSTRAINT chk_order_item_quantity_positive CHECK (quantity > 0);

ALTER TABLE terranova.order_item DROP CONSTRAINT IF EXISTS chk_order_item_unit_price_positive;
ALTER TABLE terranova.order_item ADD CONSTRAINT chk_order_item_unit_price_positive CHECK (unit_price >= 0);

-- payment
ALTER TABLE terranova.payment DROP CONSTRAINT IF EXISTS chk_payment_amount_positive;
ALTER TABLE terranova.payment ADD CONSTRAINT chk_payment_amount_positive CHECK (amount >= 0);

-- NOTE: Cannot use CONCURRENTLY inside a transaction block (this script wraps everything in BEGIN/COMMIT).
CREATE UNIQUE INDEX IF NOT EXISTS uq_payment_transaction_reference ON terranova.payment (transaction_reference) WHERE transaction_reference IS NOT NULL;

-- product
ALTER TABLE terranova.product DROP CONSTRAINT IF EXISTS chk_product_price_positive;
ALTER TABLE terranova.product ADD CONSTRAINT chk_product_price_positive CHECK (price >= 0);

ALTER TABLE terranova.product DROP CONSTRAINT IF EXISTS chk_product_stock_positive;
ALTER TABLE terranova.product ADD CONSTRAINT chk_product_stock_positive CHECK (stock >= 0);

ALTER TABLE terranova.product DROP CONSTRAINT IF EXISTS chk_product_weight_positive;
ALTER TABLE terranova.product ADD CONSTRAINT chk_product_weight_positive CHECK (weight IS NULL OR weight >= 0);

-- product_variant
ALTER TABLE terranova.product_variant DROP CONSTRAINT IF EXISTS chk_product_variant_price_adjustment_positive;
ALTER TABLE terranova.product_variant ADD CONSTRAINT chk_product_variant_price_adjustment_positive CHECK (price_adjustment >= 0);

ALTER TABLE terranova.product_variant DROP CONSTRAINT IF EXISTS chk_product_variant_stock_positive;
ALTER TABLE terranova.product_variant ADD CONSTRAINT chk_product_variant_stock_positive CHECK (stock >= 0);

-- Sub_category
ALTER TABLE terranova.sub_category DROP CONSTRAINT IF EXISTS uq_sub_category_name_per_category;
ALTER TABLE terranova.sub_category ADD CONSTRAINT uq_sub_category_name_per_category UNIQUE (name, category_id);

-- users
ALTER TABLE terranova.users DROP CONSTRAINT IF EXISTS chk_users_email_address_format;
ALTER TABLE terranova.users ADD CONSTRAINT chk_users_email_address_format CHECK (email_address ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$');

ALTER TABLE terranova.users DROP CONSTRAINT IF EXISTS chk_users_phone_number_format;
ALTER TABLE terranova.users ADD CONSTRAINT chk_users_phone_number_format CHECK (phone_number IS NULL OR phone_number ~ '^\+?[0-9]{7,15}$');

ALTER TABLE terranova.users DROP CONSTRAINT IF EXISTS chk_users_gender_valid;
ALTER TABLE terranova.users ADD CONSTRAINT chk_users_gender_valid CHECK (gender IS NULL OR gender IN ('M', 'F', 'O'));

-- refresh_tokens
CREATE UNIQUE INDEX IF NOT EXISTS uq_refresh_tokens_jti ON terranova.refresh_tokens (jti) WHERE jti IS NOT NULL;

CREATE UNIQUE INDEX IF NOT EXISTS uq_refresh_tokens_user_not_revoked ON terranova.refresh_tokens (user_id) WHERE is_revoked = FALSE;

-- 5. Funcciones/Functions

-- Update general field updated_at
CREATE OR REPLACE FUNCTION terranova.set_updated_at()
RETURNS trigger AS $$
BEGIN
  IF NEW IS DISTINCT FROM OLD THEN
    NEW.updated_at = now();
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;


-- 6. Disparadores/Triggers

-- cart
DROP TRIGGER IF EXISTS trg_cart_updated_at ON terranova.cart;
CREATE TRIGGER trg_cart_updated_at BEFORE UPDATE ON terranova.cart FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- cart_item
DROP TRIGGER IF EXISTS trg_cart_item_updated_at ON terranova.cart_item;
CREATE TRIGGER trg_cart_item_updated_at BEFORE UPDATE ON terranova.cart_item FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- category
DROP TRIGGER IF EXISTS trg_category_updated_at ON terranova.category;
CREATE TRIGGER trg_category_updated_at BEFORE UPDATE ON terranova.category FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- invoice
DROP TRIGGER IF EXISTS trg_invoice_updated_at ON terranova.invoice;
CREATE TRIGGER trg_invoice_updated_at BEFORE UPDATE ON terranova.invoice FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- invoice_item
DROP TRIGGER IF EXISTS trg_invoice_item_updated_at ON terranova.invoice_item;
CREATE TRIGGER trg_invoice_item_updated_at BEFORE UPDATE ON terranova.invoice_item FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- invoice_status
DROP TRIGGER IF EXISTS trg_invoice_status_updated_at ON terranova.invoice_status;
CREATE TRIGGER trg_invoice_status_updated_at BEFORE UPDATE ON terranova.invoice_status FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- orders
DROP TRIGGER IF EXISTS trg_orders_updated_at ON terranova.orders;
CREATE TRIGGER trg_orders_updated_at BEFORE UPDATE ON terranova.orders FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- order_item
DROP TRIGGER IF EXISTS trg_order_item_updated_at ON terranova.order_item;
CREATE TRIGGER trg_order_item_updated_at BEFORE UPDATE ON terranova.order_item FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- order_status
DROP TRIGGER IF EXISTS trg_order_status_updated_at ON terranova.order_status;
CREATE TRIGGER trg_order_status_updated_at BEFORE UPDATE ON terranova.order_status FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- payment
DROP TRIGGER IF EXISTS trg_payment_updated_at ON terranova.payment;
CREATE TRIGGER trg_payment_updated_at BEFORE UPDATE ON terranova.payment FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- payment_method
DROP TRIGGER IF EXISTS trg_payment_method_updated_at ON terranova.payment_method;
CREATE TRIGGER trg_payment_method_updated_at BEFORE UPDATE ON terranova.payment_method FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- payment_status
DROP TRIGGER IF EXISTS trg_payment_status_updated_at ON terranova.payment_status;
CREATE TRIGGER trg_payment_status_updated_at BEFORE UPDATE ON terranova.payment_status FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- payment_webhook_log
DROP TRIGGER IF EXISTS trg_payment_webhook_log_updated_at ON terranova.payment_webhook_log;
CREATE TRIGGER trg_payment_webhook_log_updated_at BEFORE UPDATE ON terranova.payment_webhook_log FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- product
DROP TRIGGER IF EXISTS trg_product_updated_at ON terranova.product;
CREATE TRIGGER trg_product_updated_at BEFORE UPDATE ON terranova.product FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- product_image
DROP TRIGGER IF EXISTS trg_product_image_updated_at ON terranova.product_image;
CREATE TRIGGER trg_product_image_updated_at BEFORE UPDATE ON terranova.product_image FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- product_variant
DROP TRIGGER IF EXISTS trg_product_variant_updated_at ON terranova.product_variant;
CREATE TRIGGER trg_product_variant_updated_at BEFORE UPDATE ON terranova.product_variant FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- product_variant_image
DROP TRIGGER IF EXISTS trg_product_variant_image_updated_at ON terranova.product_variant_image;
CREATE TRIGGER trg_product_variant_image_updated_at BEFORE UPDATE ON terranova.product_variant_image FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- shipping_address
DROP TRIGGER IF EXISTS trg_shipping_address_updated_at ON terranova.shipping_address;
CREATE TRIGGER trg_shipping_address_updated_at BEFORE UPDATE ON terranova.shipping_address FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- sub_category
DROP TRIGGER IF EXISTS trg_sub_category_updated_at ON terranova.sub_category;
CREATE TRIGGER trg_sub_category_updated_at BEFORE UPDATE ON terranova.sub_category FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- users
DROP TRIGGER IF EXISTS trg_users_updated_at ON terranova.users;
CREATE TRIGGER trg_users_updated_at BEFORE UPDATE ON terranova.users FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- wish_list
DROP TRIGGER IF EXISTS trg_wish_list_updated_at ON terranova.wish_list;
CREATE TRIGGER trg_wish_list_updated_at BEFORE UPDATE ON terranova.wish_list FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- wish_list_item
DROP TRIGGER IF EXISTS trg_wish_list_item_updated_at ON terranova.wish_list_item;
CREATE TRIGGER trg_wish_list_item_updated_at BEFORE UPDATE ON terranova.wish_list_item FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

COMMIT;
