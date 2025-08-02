import os
from pwinput import pwinput

def userinput():
	name, password = '', ''
	if 'profiles.txt' in os.listdir():
		with open('profiles.txt', 'r') as f:
			data = []
			for line in f:
				line = line.strip().split()
				data.append((line[0], line[1]))
			print('Dostępne profile:')
			print('0. Utwórz nowy')
			for i, (name, _) in enumerate(data):
				print(f'{i + 1}. {name}')
			choice = int(input('Wybierz profil (numer): '))
			if choice == 0:
				name, password = read_login_data()
			elif 0 < choice < len(data) + 1:
				name, password = data[choice - 1]
			else:
				print('Nieprawidłowy wybór. Używam domyślnego profilu.')
				name, password = data[0]
	else:
		name, password = read_login_data()
	return name, password

def read_login_data() -> tuple[str, str]:
	name = input('Podaj login lub email: ')
	password = pwinput('Podaj hasło: ') 	
	save_data = input('Czy zapisać dane logowania? (t/n): ').strip().lower()
	if save_data == 't':
		with open('profiles.txt', 'a') as f:
			f.write(f'{name} {password}\n')
	return name, password