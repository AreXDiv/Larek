create table brand (
	id integer primary key generated by default as identity,
	name text not NULL
);
insert into brand (name) values
('Байкал'),
('Беломор'),
('Зенит'),
('Столичная'),
('Победа');

select * from brand;
