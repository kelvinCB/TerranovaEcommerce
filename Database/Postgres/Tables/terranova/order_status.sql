CREATE TABLE terranova.order_status (
  id char(26) PRIMARY KEY,
  name varchar(50) NOT NULL,
  description varchar(255),
  created_at timestamptz,
  update_at timestamptz,
  is_deleted boolean NOT NULL
);