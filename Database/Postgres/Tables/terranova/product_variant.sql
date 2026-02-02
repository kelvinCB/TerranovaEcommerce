CREATE TABLE terranova.product_variant (
  id char(26) PRIMARY KEY,
  product_id char(26) NOT NULL,
  name varchar(100) NOT NULL,
  value varchar(100) NOT NULL,
  price_adjustment numeric(18, 2),
  stock integer NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz,
  is_deleted boolean NOT NULL DEFAULT FALSE
);

ALTER TABLE terranova.product_variant ADD CONSTRAINT fk_product_variant_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);