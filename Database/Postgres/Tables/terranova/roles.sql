CREATE TABLE terranova.roles (
  id char(26) PRIMARY KEY,
  name varchar(50) NOT NULL UNIQUE,
  description varchar(200),
  created_at timestamptz NOT NULL DEFAULT now()
);