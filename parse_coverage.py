import xml.etree.ElementTree as ET
import glob
import os

for path in glob.glob('src/**/TestResults/*/coverage.cobertura.xml', recursive=True):
    try:
        tree = ET.parse(path)
        root = tree.getroot()
        line_rate = float(root.attrib.get('line-rate', 0)) * 100
        lines_covered = root.attrib.get('lines-covered', 0)
        lines_valid = root.attrib.get('lines-valid', 0)
        
        # also print package coverages
        print(f"\n{os.path.basename(os.path.dirname(os.path.dirname(path)))}: Overall {line_rate:.2f}% ({lines_covered}/{lines_valid} lines)")
        
        for pkg in root.findall('.//package'):
            pkg_name = pkg.attrib.get('name')
            pkg_rate = float(pkg.attrib.get('line-rate', 0)) * 100
            print(f"  {pkg_name}: {pkg_rate:.2f}%")
            
    except Exception as e:
        pass
