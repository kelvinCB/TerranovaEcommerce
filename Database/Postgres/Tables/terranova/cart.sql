CREATE TABLE terranova.cart (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  create_at timestamptz
);

ALTER TABLE terranova.cart ADD CONSTRAINT fk_cart_user FOREIGN KEY (user_id) REFERENCES terranova.users (id);