CREATE TABLE terranova.sub_category (
  id char(26) PRIMARY KEY,
  name varchar(100) NOT NULL,
  description text,
  category_id char(26) NOT NULL,
  created_at timestamptz,
  update_at timestamptz,
  is_deleted boolean NOT NULL
);

ALTER TABLE terranova.sub_category ADD CONSTRAINT fk_sub_category_category FOREIGN KEY (category_id) REFERENCES terranova.category (id);