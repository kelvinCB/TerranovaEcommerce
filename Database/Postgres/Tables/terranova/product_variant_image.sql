CREATE TABLE terranova.product_variant_image (
  id char(26) PRIMARY KEY,
  product_variant_id char(26) NOT NULL,
  image_url varchar(255) NOT NULL,
  is_main boolean NOT NULL,
  created_at timestamptz,
  update_at timestamptz,
  is_deleted boolean NOT NULL
);

ALTER TABLE terranova.product_variant_image ADD CONSTRAINT fk_product_variant_image_product_variant FOREIGN KEY (product_variant_id) REFERENCES terranova.product_variant (id);