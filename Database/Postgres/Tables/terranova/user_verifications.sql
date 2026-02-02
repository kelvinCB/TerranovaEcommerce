CREATE TABLE terranova.user_verifications (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  purpose varchar(30) NOT NULL,            -- email_verify, reset_password, mfa
  code_hash varchar(255) NOT NULL,
  expires_at timestamptz NOT NULL,
  consumed_at timestamptz,
  created_at timestamptz NOT NULL DEFAULT now()
);

ALTER TABLE terranova.user_verifications ADD CONSTRAINT fk_user_verifications_user FOREIGN KEY (user_id) REFERENCES terranova.users(id)