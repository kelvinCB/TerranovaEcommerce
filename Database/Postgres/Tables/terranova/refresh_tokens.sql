CREATE TABLE terranova.refresh_tokens (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  token_hash varchar(255) NOT NULL,        -- hash del refresh token
  jti varchar(100),                        -- JWT ID (opcional)
  expires_at timestamptz NOT NULL,
  revoked_at timestamptz,
  replaced_by_token_id char(26),
  created_at timestamptz NOT NULL DEFAULT now(),
  user_agent varchar(200),
  ip_address varchar(45),
  CONSTRAINT fk_refresh_tokens_user FOREIGN KEY (user_id) REFERENCES terranova.users(id),
  CONSTRAINT fk_refresh_tokens_replaced FOREIGN KEY (replaced_by_token_id) REFERENCES terranova.refresh_tokens(id)
);