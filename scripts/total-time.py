"""
total-time.py

Author: Caleb Pace
Date: 31/01/2025
License: MIT

Description:
    This script normalizes times in the TimeLog.md file and updates the total time
    in the README.md file.
"""

import re, os

def is_pattern_match(s: str, suffix: str) -> bool:
    # Check if string s matches the pattern of digits followed by the given suffix
    pattern = f"^~?\\d+{suffix}$"
    return bool(re.match(pattern, s))

def parse_int(value: str) -> int:
    # Convert the integer part from the string, excluding the last character
    try:
        if (len(value) >= 1 and value[0] == '~'):
            value = value[1:] # Ignore approximation symbol

        return int(value[:-1])
    except ValueError:
        return 0  # Return 0 if conversion fails

def time_normalization(hrs: int, mins: int):
    # Get time in minutes
    mins += (hrs * 60)
    hrs = 0

    # Store sign for handling negative times
    sign_multiplier = 1
    if (mins < 0):
        sign_multiplier = -1
        mins *= sign_multiplier # Convert to positive

    # Normalize time into hours and minutes
    hrs += mins // 60
    mins = mins % 60

    # Restore sign
    hrs *= sign_multiplier
    mins *= sign_multiplier

    return hrs, mins

def time_to_string(hrs: int, mins: int) -> str:
    s = ""

    has_hrs = (hrs != 0)
    has_mins = (mins != 0)

    if has_hrs:
        s += f"{hrs}h"
    if (has_hrs and has_mins):
        s += " "
    if has_mins:
        s += f"{mins}m"

    return s

def organise_line_times(line: str):
    hrs = 0
    mins = 0
    unknown = ""
    has_unknowns = False

    # Separate title and data
    title = ""
    if ':' in line:
        title = f"{line.split(':')[0]}: " # Extract the title and ignore it
    times_str = line[len(title):].replace('#', '').strip() # Remove leading & trailing whitespace

    # Process line
    sign_multiplier = 1
    for segment in times_str.split():
        if (segment.startswith("-")):
            sign_multiplier = -1

        is_sign = (segment.startswith("+")) or (segment.startswith("-"))
        if is_sign:
            if len(segment) == 1:
                continue # Skip if just sign

            segment = segment[1:]  # Remove the first character

        has_hours = is_pattern_match(segment, 'h')
        has_minutes = is_pattern_match(segment, 'm')

        if has_hours ^ has_minutes:
            num = parse_int(segment) # Parse integer

            # Add time accordingly
            if has_hours:
                hrs += (num * sign_multiplier)
            elif has_minutes:
                mins += (num * sign_multiplier)
            
            sign_multiplier = 1 # Reset sign multiplier
        else:
            # Non-time segment
            unknown += f" {segment}"
            sign_multiplier = 1

    # Normalise time
    hrs, mins = time_normalization(hrs, mins)

    # Update the time entry
    is_time_entry = (hrs != 0 or mins != 0)
    if is_time_entry:
        # Are unknown items present
        has_unknowns = (len(unknown) > 0)
        if has_unknowns:
            unknown = f" #{unknown}"

        line = f"{title}{time_to_string(hrs, mins)}{unknown}\n"

    return line, is_time_entry, has_unknowns, hrs, mins

def total_times(time_log_filepath: str, total_filepath: str, parent_directory: str):
    skip_n_data_lines = 0
    total_hrs = 0
    total_mins = 0

    # Store time log data
    with open(time_log_filepath, 'r') as file:
        lines = file.readlines()

    # Process lines
    for i in range(len(lines)):
        lines[i], is_time_entry, has_unknowns, hrs, mins = organise_line_times(lines[i])

        if has_unknowns:
            print(f"  ! line {(i + 1)} has unknown time values.")

        if (is_time_entry):
            # Skip lines if needed
            if skip_n_data_lines > 0:
                skip_n_data_lines -= 1
                break
            
            total_hrs += hrs
            total_mins += mins

    # Update time log
    with open(time_log_filepath, 'w') as file:
        file.writelines(lines)

    # Read the total file to update
    with open(total_filepath, 'r') as file:
        lines = file.readlines()

    # Create total string
    total_hrs, total_mins = time_normalization(total_hrs, total_mins)
    total_str = f"Total: {time_to_string(total_hrs, total_mins)}\n"

    # Update total line
    total_line_num = 0
    for i in range(len(lines)):
        if bool(re.match(r"^Total: ([\d-]+h)?( )?([\d-]+m)?$", lines[i])):
            lines[i] = total_str     # Update the total line
            total_line_num = (i + 1) # Store the line number (1-based index)
            break

    # If no total line is found, print a warning
    if (total_line_num == 0):
        print(f"\n  ! Total line was not found.")
        return

    # Write updated lines to total file
    with open(total_filepath, 'w') as file:
        file.writelines(lines)

    # Display total time
    print(f"\nTotalled \"{os.path.relpath(time_log_filepath, parent_directory)}\" entries.\n"
        + f"'{total_str[:-1]}' -> \"{os.path.relpath(total_filepath, parent_directory)}:{total_line_num}\"")

def main():
    parent_directory = os.getcwd()
    time_log_filepath = os.path.join(parent_directory, "TimeLog.md")
    total_filepath = os.path.join(parent_directory, "README.md")

    total_times(time_log_filepath, total_filepath, parent_directory)

if __name__ == "__main__":
    main()