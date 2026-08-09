import re
import sys

def extract_methods(content, regex):
    return set(re.findall(regex, content))

rust_methods = set()
cs_methods = set()

with open('/home/philip/githome/teaql-rs/teaql-sql/src/dialect.rs') as f:
    rust_methods.update(re.findall(r'fn\s+([a-zA-Z0-9_]+)\s*\(', f.read()))

with open('/home/philip/githome/teaql-dotnet/src/TeaQL.Sql/Dialect.cs') as f:
    cs_methods.update(re.findall(r'(?:public|private|protected|internal)\s+(?:virtual\s+|override\s+|static\s+|async\s+)*[\w\<\>\[\]]+\s+([A-Z][a-zA-Z0-9_]*)\s*\(', f.read()))

def to_pascal(snake):
    return ''.join(word.title() for word in snake.split('_'))

missing = []
for m in rust_methods:
    if to_pascal(m) not in cs_methods and m not in ['new', 'default']:
        missing.append(m)

print("Dialect Missing:", missing)
