CREATE TABLE terranova.shipping_address (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  address varchar(200) NOT NULL,
  city varchar(50) NOT NULL,
  state varchar(50),
  country varchar(50) NOT NULL,
  postal_code varchar(20),
  phone_number varchar(20),
  created_at timestamptz,
  update_at timestamptz,
  is_deleted boolean NOT NULL
);

ALTER TABLE terranova.shipping_address ADD CONSTRAINT fk_shipping_address_user FOREIGN KEY (user_id) REFERENCES terranova.users (id);