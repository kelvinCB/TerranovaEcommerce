CREATE TABLE terranova.shipping_address (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  address1 varchar(200) NOT NULL,
  adress2 varchar(200),
  city varchar(50) NOT NULL,
  state varchar(50),
  country varchar(50) NOT NULL,
  postal_code varchar(20),
  phone_number varchar(20),
  is_default boolean NOT NULL DEFAULT FALSE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz,
  is_deleted boolean NOT NULL DEFAULT FALSE
);

ALTER TABLE terranova.shipping_address ADD CONSTRAINT fk_shipping_address_user FOREIGN KEY (user_id) REFERENCES terranova.users (id);
CREATE UNIQUE INDEX only_one_default_shipping_address ON terranova.shipping_address(user_id) WHERE is_default = TRUE;