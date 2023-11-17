create table orders(
	id integer primary key generated by default as identity,
	product_id integer not NULL,
	count integer not NULL check (count > 0),
	delivery boolean default false,
	payment boolean default false,
	customer_name text not NULL,
	summ double precision NOT NULL
);

