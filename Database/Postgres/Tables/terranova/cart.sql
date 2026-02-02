CREATE TABLE terranova.cart (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL UNIQUE,
  created_at timestamptz NOT NULL DEFAULT NOW()
);

ALTER TABLE terranova.cart ADD CONSTRAINT fk_cart_user FOREIGN KEY (user_id) REFERENCES terranova.users (id);