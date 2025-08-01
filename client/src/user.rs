/// File where the username is stored
const USER_FILE: &str = "user.txt";

/// Prompts the user to create a new username, validates it, and saves it to file.
fn create_user() -> String {
    use std::io::{self, Write};

    println!("Please enter a username:    ([a-zA-Z0-9_.-], 10 characters max)");

    loop {
        print!("> ");
        io::stdout().flush().unwrap(); // Ensure prompt is shown

        let mut input = String::new();
        io::stdin().read_line(&mut input).expect("Failed to read line");

        // Filter input to allowed characters and limit to 10 chars
        let username: String = input
            .trim()
            .chars()
            .filter(|c| c.is_alphanumeric() || ['_', '.', '-'].contains(c))
            .take(10)
            .collect();

        if username.is_empty() {
            println!("Invalid username. Please try again.");
            continue;
        }

        // Save valid username to file
        save_user(username.clone()).expect("failed to save user");

        return username;
    }
}

/// Saves the username to USER_FILE.
fn save_user(username: String) -> std::io::Result<()> {
    use std::fs::File;
    use std::io::Write;

    let mut file = File::create(USER_FILE)?;
    file.write_all(username.as_bytes())?;
    Ok(())
}

/// Loads the username from USER_FILE, returns None if not found.
fn load_user() -> Option<String> {
    use std::fs;

    fs::read_to_string(USER_FILE).ok()
}

/// Returns the saved username, or prompts to create one if not found.
pub fn resolve_user() -> String {
    if let Some(username) = load_user() {
        username
    } else {
        create_user()
    }
}
