CREATE TABLE terranova.product_variant (
  id char(26) PRIMARY KEY,
  product_id char(26) NOT NULL,
  name varchar(100) NOT NULL,
  value varchar(100) NOT NULL,
  price_adjustment numeric(18, 2),
  stock char(26),
  created_at timestamptz,
  update_at timestamptz,
  is_deleted boolean NOT NULL
);

ALTER TABLE terranova.product_variant ADD CONSTRAINT fk_product_variant_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);