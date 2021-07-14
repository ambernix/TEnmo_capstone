
DELETE FROM transfers
DELETE FROM accounts
DELETE FROM users

SET IDENTITY_INSERT users ON
INSERT INTO users (user_id, username, password_hash, salt)
VALUES	(1, 'test', 'nDNCEP96xuKaLZ1NXtDG7M9BlNM=', 'e5HRpf8/5T4='),
		(2, 'test2', 'C/bsSQGnwRkXVtjclFGA94wpstw=', 'eBXsMsMfzc8=')
SET IDENTITY_INSERT users OFF

SET IDENTITY_INSERT accounts ON
INSERT INTO accounts (account_id, user_id, balance)
VALUES	(11, 1, 990),
		(12, 1, 10),
		(13, 2, 1010)
SET IDENTITY_INSERT accounts OFF

SET IDENTITY_INSERT transfers ON
INSERT INTO transfers	(transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount)
VALUES					(21, 2, 2, 11, 13, 10),
						(22, 2, 2, 11, 13, 11.11),
						(23, 1, 1, 13, 11, 100),
						(24, 1, 1, 12, 13, 11)
SET IDENTITY_INSERT transfers OFF
