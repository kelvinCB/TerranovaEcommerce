CREATE TABLE terranova.product_image (
  id char(26) PRIMARY KEY,
  product_id char(26) NOT NULL,
  image_url varchar(255) NOT NULL,
  is_main boolean NOT NULL,
  created_at timestamptz,
  update_at timestamptz,
  is_deleted boolean NOT NULL
);

ALTER TABLE terranova.product_image ADD CONSTRAINT fk_product_image_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);